using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using VladislavTsurikov.ToolSystem.Editor.UIToolkit;
using VladislavTsurikov.ToolSystem.Runtime.Core;

namespace VladislavTsurikov.PackageJsonSync.Editor
{
    public class PackageJsonSyncWindow : EditorWindow
    {
        private ToolStack _toolStack;
        private ToolStackEditor _toolsEditor;

        [MenuItem("Window/Universal Toolkit/Package.json Sync")]
        public static void ShowWindow()
        {
            var window = GetWindow<PackageJsonSyncWindow>("Package.json Sync");
            window.minSize = new Vector2(800, 600);
            window.Show();
        }

        private void OnEnable()
        {
            _toolStack = new ToolStack();
            _toolsEditor = new ToolStackEditor(_toolStack);
        }

        private void CreateGUI()
        {
            var root = rootVisualElement;
            root.style.flexDirection = FlexDirection.Column;

            CreateHeader(root);

            var toolsContainer = _toolsEditor.CreateVisualElement();
            toolsContainer.style.flexGrow = 1;

            root.Add(toolsContainer);
        }

        private void CreateHeader(VisualElement root)
        {
            var header = new VisualElement();
            header.style.backgroundColor = new Color(0.2f, 0.2f, 0.2f);
            header.style.paddingTop = 15;
            header.style.paddingBottom = 15;
            header.style.paddingLeft = 20;
            header.style.paddingRight = 20;
            header.style.marginBottom = 10;

            var title = new Label("Package.json Sync");
            title.style.fontSize = 20;
            title.style.unityFontStyleAndWeight = FontStyle.Bold;
            title.style.marginBottom = 5;
            header.Add(title);

            var description = new Label(
                "Generate UPM-compliant package.json files for your git repositories.\n" +
                "Add tools below and click on them to execute.");
            description.style.fontSize = 12;
            description.style.color = new Color(0.7f, 0.7f, 0.7f);
            description.style.whiteSpace = WhiteSpace.Normal;
            header.Add(description);

            root.Add(header);
        }
    }
}
