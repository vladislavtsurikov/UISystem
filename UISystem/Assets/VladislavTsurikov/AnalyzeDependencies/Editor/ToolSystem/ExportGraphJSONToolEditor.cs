using UnityEngine.UIElements;
using VladislavTsurikov.ComponentStack.Editor.Core;
using VladislavTsurikov.Core.Editor;
using VladislavTsurikov.ToolSystem.Editor.UIToolkit;

namespace VladislavTsurikov.AnalyzeDependencies.Editor.ToolSystem
{
    [ElementEditor(typeof(ExportGraphJSONTool))]
    public class ExportGraphJSONToolEditor : UIToolkitToolEditor
    {
        private ExportGraphJSONTool Tool => (ExportGraphJSONTool)Target;

        protected override VisualElement CreateVisualElement()
        {
            var container = new VisualElement();
            container.style.paddingTop = 10;
            container.style.paddingBottom = 10;
            container.style.paddingLeft = 10;
            container.style.paddingRight = 10;

            var helpBox = new HelpBox(
                "Export your dependency graph to JSON format.\n\n" +
                "The JSON includes:\n" +
                "• Node data with assembly names and metrics\n" +
                "• Edge data showing dependencies\n" +
                "• Statistics about the graph\n\n" +
                "Compatible with D3.js, vis.js, Cytoscape.",
                HelpBoxMessageType.Info);
            container.Add(helpBox);

            // Export button
            var exportButton = new Button(() => Tool.Export());
            exportButton.text = "Export to JSON Format";
            exportButton.style.height = 30;
            exportButton.style.marginTop = 10;
            container.Add(exportButton);

            return container;
        }
    }
}
