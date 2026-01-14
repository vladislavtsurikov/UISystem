using System.IO;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.AnalyzeDependencies.Editor.Core;
using VladislavTsurikov.AnalyzeDependencies.Editor.Core.Graph;
using VladislavTsurikov.AttributeUtility.Runtime;
using VladislavTsurikov.ToolSystem.Runtime.Core;

namespace VladislavTsurikov.AnalyzeDependencies.Editor.ToolSystem
{
    [Name("Dependency Analyzer/Export Visualization (HTML)")]
    [Tool("Export Visualization (HTML)", "Export interactive D3.js visualization to HTML")]
    [ToolGroup("Dependencies")]
    public class ExportGraphHTMLTool : EditorTool
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
                "Export Dependency Visualization (HTML)",
                Application.dataPath,
                "dependency_graph.html",
                "html");

            if (string.IsNullOrEmpty(path))
                return;

            try
            {
                EditorUtility.DisplayProgressBar("Exporting", "Generating interactive visualization...", 0.5f);

                var graphGenerator = new DependencyGraphGenerator(analyzer);
                string htmlContent = graphGenerator.ExportToHTMLVisualization();
                File.WriteAllText(path, htmlContent);

                if (EditorUtility.DisplayDialog("Export Complete",
                    $"Interactive dependency visualization exported to:\n{path}\n\n" +
                    "This is a standalone HTML file with:\n" +
                    "- Interactive D3.js force-directed graph\n" +
                    "- Nodes sized and colored by centrality\n" +
                    "- Core dependencies in center, edge dependencies on periphery\n" +
                    "- Zoom, pan, and drag functionality\n\n" +
                    "Open in browser now?",
                    "Open in Browser", "Just Show File"))
                {
                    Application.OpenURL("file://" + path);
                }
                else
                {
                    EditorUtility.RevealInFinder(path);
                }
            }
            catch (System.Exception e)
            {
                EditorUtility.DisplayDialog("Export Failed", $"Failed to export visualization: {e.Message}", "OK");
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }
        }
    }
}
