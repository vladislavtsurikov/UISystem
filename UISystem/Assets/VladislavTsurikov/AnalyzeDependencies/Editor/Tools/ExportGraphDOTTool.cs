using System.IO;
using UnityEditor;
using VladislavTsurikov.AnalyzeDependencies.Editor.Core;
using VladislavTsurikov.AnalyzeDependencies.Editor.Core.Graph;
using VladislavTsurikov.AnalyzeDependencies.Editor.Core.Models;
using UnityEngine;

namespace VladislavTsurikov.AnalyzeDependencies.Editor.Tools
{
    public class ExportGraphDOTTool : DependencyTool
    {
        public override string Name => "Export Graph (DOT)";
        public override string Description => "Export dependency graph to Graphviz DOT format";

        public override bool CanExecute(DependencyAnalyzer analyzer) => analyzer.GetAllAssemblies().Count > 0;

        public override void Execute(DependencyAnalyzer analyzer)
        {
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
                EditorUtility.DisplayDialog("Export Failed", $"Failed to export graph: {e.Message}", "OK");
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }
        }
    }
}
