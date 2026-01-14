using System.IO;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.AnalyzeDependencies.Editor.Core;
using VladislavTsurikov.AnalyzeDependencies.Editor.Core.Graph;
using VladislavTsurikov.AttributeUtility.Runtime;
using VladislavTsurikov.ToolSystem.Runtime.Core;

namespace VladislavTsurikov.AnalyzeDependencies.Editor.ToolSystem
{
    [Name("Dependency Analyzer/Export Graph (DOT)")]
    [Tool("Export Graph (DOT)", "Export dependency graph to Graphviz DOT format")]
    [ToolGroup("Dependencies")]
    public class ExportGraphDOTTool : EditorTool
    {
        protected override void OnSetupTool()
        {
            // OnSetupTool does nothing for export tools
            // Export is triggered from UI button
        }

        public void Export()
        {
            var analyzer = DependencyAnalyzerInitialize.Instance;

            string path = EditorUtility.SaveFilePanel(
                "Export Dependency Graph (DOT)",
                Application.dataPath,
                "dependency_graph.dot",
                "dot");

            if (string.IsNullOrEmpty(path))
                return;

            try
            {
                EditorUtility.DisplayProgressBar("Exporting", "Generating dependency graph...", 0.5f);

                var graphGenerator = new DependencyGraphGenerator(analyzer);
                string dotContent = graphGenerator.ExportToDOT();
                File.WriteAllText(path, dotContent);

                EditorUtility.ClearProgressBar();

                EditorUtility.DisplayDialog("Export Complete",
                    $"Dependency graph exported to:\n{path}\n\n" +
                    "You can visualize this file using Graphviz or online tools like:\n" +
                    "- https://dreampuf.github.io/GraphvizOnline/\n" +
                    "- https://www.webgraphviz.com/",
                    "OK");

                EditorUtility.RevealInFinder(path);
            }
            catch (System.Exception e)
            {
                EditorUtility.ClearProgressBar();
                EditorUtility.DisplayDialog("Export Failed", $"Failed to export graph: {e.Message}", "OK");
            }
        }
    }
}
