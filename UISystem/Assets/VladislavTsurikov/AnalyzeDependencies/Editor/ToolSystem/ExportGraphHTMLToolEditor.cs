using UnityEngine.UIElements;
using VladislavTsurikov.Core.Editor;
using VladislavTsurikov.ToolSystem.Editor.UIToolkit;

namespace VladislavTsurikov.AnalyzeDependencies.Editor.ToolSystem
{
    [ElementEditor(typeof(ExportGraphHTMLTool))]
    public class ExportGraphHTMLToolEditor : UIToolkitToolEditor
    {
        private ExportGraphHTMLTool Tool => (ExportGraphHTMLTool)Target;

        protected override VisualElement CreateVisualElement()
        {
            var container = new VisualElement();
            container.style.paddingTop = 10;
            container.style.paddingBottom = 10;
            container.style.paddingLeft = 10;
            container.style.paddingRight = 10;

            var helpBox = new HelpBox(
                "Export an interactive HTML visualization.\n\n" +
                "Features:\n" +
                "• Force-directed graph layout with D3.js\n" +
                "• Nodes sized and colored by centrality\n" +
                "• Interactive zoom, pan, and drag\n" +
                "• Standalone HTML file (no server needed)",
                HelpBoxMessageType.Info);
            container.Add(helpBox);

            // Export button
            var exportButton = new Button(() => Tool.Export());
            exportButton.text = "Export to HTML Format";
            exportButton.style.height = 30;
            exportButton.style.marginTop = 10;
            container.Add(exportButton);

            return container;
        }
    }
}
