using System.IO;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.AnalyzeDependencies.Editor.Core;
using VladislavTsurikov.AnalyzeDependencies.Editor.Core.Graph;
using VladislavTsurikov.AttributeUtility.Runtime;
using VladislavTsurikov.ToolSystem.Runtime.Core;

namespace VladislavTsurikov.AnalyzeDependencies.Editor.ToolSystem
{
    [Name("Dependency Analyzer/Export Graph (JSON)")]
    [Tool("Export Graph (JSON)", "Export dependency graph to JSON format")]
    [ToolGroup("Dependencies")]
    public class ExportGraphJSONTool : EditorTool
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
                "Export Dependency Graph (JSON)",
                Application.dataPath,
                "dependency_graph.json",
                "json");

            if (string.IsNullOrEmpty(path))
                return;

            try
            {
                EditorUtility.DisplayProgressBar("Exporting", "Generating dependency graph...", 0.5f);

                var graphGenerator = new DependencyGraphGenerator(analyzer);
                string jsonContent = graphGenerator.ExportToJSON();
                File.WriteAllText(path, jsonContent);

                EditorUtility.DisplayDialog("Export Complete",
                    $"Dependency graph exported to:\n{path}\n\n" +
                    "This JSON file contains:\n" +
                    "- Node data (assemblies with centrality metrics)\n" +
                    "- Edge data (dependencies)\n" +
                    "- Statistics\n\n" +
                    "You can use this with D3.js, vis.js, or other graph visualization libraries.",
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
