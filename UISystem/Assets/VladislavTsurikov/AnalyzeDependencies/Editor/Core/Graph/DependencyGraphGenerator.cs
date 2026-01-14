using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using VladislavTsurikov.AnalyzeDependencies.Editor.Core.Models;

namespace VladislavTsurikov.AnalyzeDependencies.Editor.Core.Graph
{
    public class DependencyGraphGenerator
    {
        private readonly DependencyAnalyzer _analyzer;
        private DependencyGraph _graph;

        public DependencyGraphGenerator(DependencyAnalyzer analyzer)
        {
            _analyzer = analyzer;
        }

        public DependencyGraph BuildGraph()
        {
            _graph = new DependencyGraph();
            var assembliesDict = _analyzer.GetAssembliesDictionary();
            var assemblies = assembliesDict.Values.ToList();

            foreach (var assembly in assemblies)
            {
                assembly.UsedBy.Clear();
                assembly.Centrality = 0;
                _graph.Nodes.Add(assembly);
            }

            foreach (var assembly in assemblies)
            {
                foreach (var depGuid in assembly.Dependencies)
                {
                    if (assembliesDict.TryGetValue(_analyzer.GetAssemblyNameByGuid(depGuid), out var depAssembly))
                    {
                        depAssembly.UsedBy.Add(assembly.Guid);

                        var edge = new DependencyEdge
                        {
                            FromGuid = assembly.Guid,
                            ToGuid = depGuid,
                            FromName = assembly.Name,
                            ToName = depAssembly.Name,
                            IsUnused = assembly.UnusedDependencies.Contains(depGuid)
                        };

                        _graph.Edges.Add(edge);

                        if (edge.IsUnused)
                        {
                            _graph.UnusedDependencies++;
                        }
                    }
                }
            }

            CalculateCentrality();

            _graph.TotalAssemblies = _graph.Nodes.Count;
            _graph.TotalDependencies = _graph.Edges.Count;

            return _graph;
        }

        private void CalculateCentrality()
        {
            if (_graph.Nodes.Count == 0)
                return;

            int maxUsage = _graph.Nodes.Max(n => n.UsageCount);
            int maxDeps = _graph.Nodes.Max(n => n.DependencyCount);

            foreach (var node in _graph.Nodes)
            {
                float usageScore = maxUsage > 0 ? (float)node.UsageCount / maxUsage : 0;
                float depPenalty = maxDeps > 0 ? 1.0f - ((float)node.DependencyCount / maxDeps * 0.5f) : 1.0f;
                node.Centrality = usageScore * 0.7f + depPenalty * 0.3f;
            }
        }

        public string ExportToDOT()
        {
            if (_graph == null)
                BuildGraph();

            StringBuilder sb = new StringBuilder();

            sb.AppendLine("digraph Dependencies {");
            sb.AppendLine("    rankdir=TB;");
            sb.AppendLine("    node [shape=box, style=rounded];");
            sb.AppendLine("    concentrate=true;");
            sb.AppendLine();

            var sortedNodes = _graph.Nodes.OrderByDescending(n => n.Centrality).ToList();

            var coreNodes = sortedNodes.Where(n => n.Centrality > 0.7f).ToList();
            var midNodes = sortedNodes.Where(n => n.Centrality > 0.3f && n.Centrality <= 0.7f).ToList();
            var edgeNodes = sortedNodes.Where(n => n.Centrality <= 0.3f).ToList();

            if (coreNodes.Any())
            {
                sb.AppendLine("    subgraph cluster_core {");
                sb.AppendLine("        label=\"Core Dependencies\";");
                sb.AppendLine("        style=filled;");
                sb.AppendLine("        color=lightgrey;");
                sb.AppendLine("        rank=same;");

                foreach (var node in coreNodes)
                {
                    string color = GetNodeColor(node);
                    sb.AppendLine($"        \"{EscapeDOT(node.Name)}\" [fillcolor=\"{color}\", style=\"filled,rounded\", tooltip=\"Used by: {node.UsageCount}, Dependencies: {node.DependencyCount}, Centrality: {node.Centrality:F2}\"];");
                }

                sb.AppendLine("    }");
                sb.AppendLine();
            }

            if (midNodes.Any())
            {
                sb.AppendLine("    subgraph cluster_mid {");
                sb.AppendLine("        label=\"Mid-tier Dependencies\";");
                sb.AppendLine("        style=filled;");
                sb.AppendLine("        color=white;");

                foreach (var node in midNodes)
                {
                    string color = GetNodeColor(node);
                    sb.AppendLine($"        \"{EscapeDOT(node.Name)}\" [fillcolor=\"{color}\", style=\"filled,rounded\", tooltip=\"Used by: {node.UsageCount}, Dependencies: {node.DependencyCount}, Centrality: {node.Centrality:F2}\"];");
                }

                sb.AppendLine("    }");
                sb.AppendLine();
            }

            if (edgeNodes.Any())
            {
                foreach (var node in edgeNodes)
                {
                    string color = GetNodeColor(node);
                    sb.AppendLine($"    \"{EscapeDOT(node.Name)}\" [fillcolor=\"{color}\", style=\"filled,rounded\", tooltip=\"Used by: {node.UsageCount}, Dependencies: {node.DependencyCount}, Centrality: {node.Centrality:F2}\"];");
                }
                sb.AppendLine();
            }

            foreach (var edge in _graph.Edges)
            {
                string style = edge.IsUnused ? "dashed" : "solid";
                string color = edge.IsUnused ? "red" : "black";
                sb.AppendLine($"    \"{EscapeDOT(edge.FromName)}\" -> \"{EscapeDOT(edge.ToName)}\" [style={style}, color={color}];");
            }

            sb.AppendLine("}");

            return sb.ToString();
        }

        public string ExportToJSON()
        {
            if (_graph == null)
                BuildGraph();

            var json = new
            {
                nodes = _graph.Nodes.OrderByDescending(n => n.Centrality).Select(n => new
                {
                    id = n.Guid,
                    name = n.Name,
                    usageCount = n.UsageCount,
                    dependencyCount = n.DependencyCount,
                    centrality = n.Centrality,
                    group = GetNodeGroup(n),
                    level = GetNodeLevel(n)
                }).ToList(),

                edges = _graph.Edges.Select(e => new
                {
                    source = e.FromGuid,
                    target = e.ToGuid,
                    isUnused = e.IsUnused
                }).ToList(),

                statistics = new
                {
                    totalAssemblies = _graph.TotalAssemblies,
                    totalDependencies = _graph.TotalDependencies,
                    unusedDependencies = _graph.UnusedDependencies
                }
            };

            return JsonUtility.ToJson(json, true);
        }

        public string ExportToHTMLVisualization()
        {
            if (_graph == null)
                BuildGraph();

            string json = ExportToJSON();

            string html = @"<!DOCTYPE html>
<html>
<head>
    <meta charset=""utf-8"">
    <title>Dependency Graph - Universal Toolkit</title>
    <script src=""https://d3js.org/d3.v7.min.js""></script>
    <style>
        body {
            margin: 0;
            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
            background: #1e1e1e;
            color: #fff;
        }

        #graph {
            width: 100vw;
            height: 100vh;
        }

        .node {
            cursor: pointer;
            stroke: #fff;
            stroke-width: 2px;
        }

        .node.core {
            fill: #4CAF50;
        }

        .node.mid {
            fill: #2196F3;
        }

        .node.edge {
            fill: #9E9E9E;
        }

        .link {
            stroke: #666;
            stroke-opacity: 0.6;
        }

        .link.unused {
            stroke: #f44336;
            stroke-dasharray: 5,5;
        }

        .node-label {
            font-size: 10px;
            pointer-events: none;
            fill: #fff;
            text-anchor: middle;
        }

        .tooltip {
            position: absolute;
            padding: 10px;
            background: rgba(0, 0, 0, 0.9);
            color: #fff;
            border-radius: 5px;
            pointer-events: none;
            font-size: 12px;
            display: none;
        }

        #info {
            position: absolute;
            top: 20px;
            left: 20px;
            background: rgba(0, 0, 0, 0.8);
            padding: 20px;
            border-radius: 10px;
            max-width: 300px;
        }

        h1 {
            margin: 0 0 10px 0;
            font-size: 20px;
        }

        .stat {
            margin: 5px 0;
            font-size: 14px;
        }

        .legend {
            margin-top: 15px;
            padding-top: 15px;
            border-top: 1px solid #444;
        }

        .legend-item {
            display: flex;
            align-items: center;
            margin: 5px 0;
            font-size: 12px;
        }

        .legend-color {
            width: 20px;
            height: 20px;
            margin-right: 10px;
            border-radius: 3px;
        }
    </style>
</head>
<body>
    <div id=""info"">
        <h1>Dependency Graph</h1>
        <div class=""stat"">Total Assemblies: <strong id=""totalAssemblies"">0</strong></div>
        <div class=""stat"">Total Dependencies: <strong id=""totalDeps"">0</strong></div>
        <div class=""stat"">Unused Dependencies: <strong id=""unusedDeps"">0</strong></div>

        <div class=""legend"">
            <div class=""legend-item"">
                <div class=""legend-color"" style=""background: #4CAF50;""></div>
                <span>Core (High Centrality)</span>
            </div>
            <div class=""legend-item"">
                <div class=""legend-color"" style=""background: #2196F3;""></div>
                <span>Mid-tier (Medium Centrality)</span>
            </div>
            <div class=""legend-item"">
                <div class=""legend-color"" style=""background: #9E9E9E;""></div>
                <span>Edge (Low Centrality)</span>
            </div>
        </div>
    </div>

    <div id=""tooltip"" class=""tooltip""></div>
    <svg id=""graph""></svg>

    <script>
        const data = " + json + @";

        document.getElementById('totalAssemblies').textContent = data.statistics.totalAssemblies;
        document.getElementById('totalDeps').textContent = data.statistics.totalDependencies;
        document.getElementById('unusedDeps').textContent = data.statistics.unusedDependencies;

        const width = window.innerWidth;
        const height = window.innerHeight;
        const svg = d3.select('#graph')
            .attr('width', width)
            .attr('height', height);

        const g = svg.append('g');

        const zoom = d3.zoom()
            .scaleExtent([0.1, 10])
            .on('zoom', (event) => {
                g.attr('transform', event.transform);
            });

        svg.call(zoom);

        const tooltip = d3.select('#tooltip');

        const simulation = d3.forceSimulation(data.nodes)
            .force('link', d3.forceLink(data.edges)
                .id(d => d.id)
                .distance(100))
            .force('charge', d3.forceManyBody()
                .strength(-300))
            .force('center', d3.forceCenter(width / 2, height / 2))
            .force('radial', d3.forceRadial(d => {
                if (d.centrality > 0.7) return 100;
                if (d.centrality > 0.3) return 300;
                return 500;
            }, width / 2, height / 2));

        const link = g.append('g')
            .selectAll('line')
            .data(data.edges)
            .enter().append('line')
            .attr('class', d => d.isUnused ? 'link unused' : 'link')
            .attr('stroke-width', 2);

        const node = g.append('g')
            .selectAll('circle')
            .data(data.nodes)
            .enter().append('circle')
            .attr('class', d => 'node ' + d.group)
            .attr('r', d => 10 + d.centrality * 20)
            .call(d3.drag()
                .on('start', dragstarted)
                .on('drag', dragged)
                .on('end', dragended))
            .on('mouseover', (event, d) => {
                tooltip
                    .style('display', 'block')
                    .html(`
                        <strong>${d.name}</strong><br/>
                        Used by: ${d.usageCount} assemblies<br/>
                        Dependencies: ${d.dependencyCount}<br/>
                        Centrality: ${d.centrality.toFixed(2)}
                    `)
                    .style('left', (event.pageX + 10) + 'px')
                    .style('top', (event.pageY - 10) + 'px');
            })
            .on('mouseout', () => {
                tooltip.style('display', 'none');
            });

        const label = g.append('g')
            .selectAll('text')
            .data(data.nodes)
            .enter().append('text')
            .attr('class', 'node-label')
            .text(d => d.name);

        simulation.on('tick', () => {
            link
                .attr('x1', d => d.source.x)
                .attr('y1', d => d.source.y)
                .attr('x2', d => d.target.x)
                .attr('y2', d => d.target.y);

            node
                .attr('cx', d => d.x)
                .attr('cy', d => d.y);

            label
                .attr('x', d => d.x)
                .attr('y', d => d.y - 20);
        });

        function dragstarted(event, d) {
            if (!event.active) simulation.alphaTarget(0.3).restart();
            d.fx = d.x;
            d.fy = d.y;
        }

        function dragged(event, d) {
            d.fx = event.x;
            d.fy = event.y;
        }

        function dragended(event, d) {
            if (!event.active) simulation.alphaTarget(0);
            d.fx = null;
            d.fy = null;
        }
    </script>
</body>
</html>";

            return html;
        }

        private string GetNodeColor(AssemblyInfo node)
        {
            if (node.Centrality > 0.7f) return "#90EE90";
            if (node.Centrality > 0.3f) return "#87CEEB";
            return "#D3D3D3";
        }

        private string GetNodeGroup(AssemblyInfo node)
        {
            if (node.Centrality > 0.7f) return "core";
            if (node.Centrality > 0.3f) return "mid";
            return "edge";
        }

        private int GetNodeLevel(AssemblyInfo node)
        {
            if (node.Centrality > 0.7f) return 1;
            if (node.Centrality > 0.3f) return 2;
            return 3;
        }

        private string EscapeDOT(string text) => text.Replace("\"", "\\\"");
    }
}
