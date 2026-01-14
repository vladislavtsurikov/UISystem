using UnityEngine.UIElements;
using VladislavTsurikov.ComponentStack.Editor.Core;
using VladislavTsurikov.Core.Editor;
using VladislavTsurikov.ToolSystem.Editor.UIToolkit;

namespace VladislavTsurikov.AnalyzeDependencies.Editor.ToolSystem
{
    [ElementEditor(typeof(ExportGraphDOTTool))]
    public class ExportGraphDOTToolEditor : UIToolkitToolEditor
    {
        private ExportGraphDOTTool Tool => (ExportGraphDOTTool)Target;

        protected override VisualElement CreateVisualElement()
        {
            var container = new VisualElement();
            container.style.paddingTop = 10;
            container.style.paddingBottom = 10;
            container.style.paddingLeft = 10;
            container.style.paddingRight = 10;

            var helpBox = new HelpBox(
                "Export your assembly dependency graph to DOT format (Graphviz).\n\n" +
                "DOT files can be visualized using:\n" +
                "• Graphviz desktop application\n" +
                "• Online tools (GraphvizOnline, WebGraphviz)\n" +
                "• VS Code extensions",
                HelpBoxMessageType.Info);
            container.Add(helpBox);

            // Export button
            var exportButton = new Button(() => Tool.Export());
            exportButton.text = "Export to DOT Format";
            exportButton.style.height = 30;
            exportButton.style.marginTop = 10;
            container.Add(exportButton);

            return container;
        }
    }
}
