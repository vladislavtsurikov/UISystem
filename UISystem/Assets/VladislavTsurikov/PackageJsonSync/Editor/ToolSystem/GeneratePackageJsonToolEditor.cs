using UnityEngine.UIElements;
using VladislavTsurikov.ComponentStack.Editor.Core;
using VladislavTsurikov.Core.Editor;
using VladislavTsurikov.ToolSystem.Editor.UIToolkit;

namespace VladislavTsurikov.PackageJsonSync.Editor.ToolSystem
{
    [ElementEditor(typeof(GeneratePackageJsonTool))]
    public class GeneratePackageJsonToolEditor : UIToolkitToolEditor
    {
        private GeneratePackageJsonTool Tool => (GeneratePackageJsonTool)Target;

        protected override VisualElement CreateVisualElement()
        {
            var container = new VisualElement();
            container.style.paddingTop = 10;
            container.style.paddingBottom = 10;
            container.style.paddingLeft = 10;
            container.style.paddingRight = 10;

            var helpBox = new HelpBox(
                "Generates a UPM-compliant package.json file.\n\n" +
                "The package.json will include:\n" +
                "• Package name and metadata\n" +
                "• External assembly dependencies\n" +
                "• Unity version requirement\n\n" +
                "Internal VladislavTsurikov dependencies are excluded.",
                HelpBoxMessageType.Info);
            container.Add(helpBox);

            // Generate button
            var generateButton = new Button(() => Tool.Generate());
            generateButton.text = "Generate package.json";
            generateButton.style.height = 30;
            generateButton.style.marginTop = 10;
            container.Add(generateButton);

            return container;
        }
    }
}
