using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using VladislavTsurikov.ToolSystem.Editor.Core;
using VladislavTsurikov.ToolSystem.Runtime.Core;

namespace VladislavTsurikov.ToolSystem.Editor.UIToolkit
{
    public class ToolSystemWindow : EditorWindow
    {
        private ToolStack _toolStack;
        private ToolStackEditor _toolStackEditor;

        [MenuItem("Tools/Vladislav Tsurikov/Tool System")]
        public static void ShowWindow()
        {
            var window = GetWindow<ToolSystemWindow>("Tool System");
            window.minSize = new Vector2(800, 600);
            window.Show();
        }

        private void OnEnable()
        {
            if (_toolStack == null)
            {
                _toolStack = new ToolStack();
            }
        }

        private void CreateGUI()
        {
            var root = rootVisualElement;
            root.style.flexDirection = FlexDirection.Column;

            CreateHeader(root);

            _toolStackEditor = new ToolStackEditor(_toolStack);
            var toolEditorElement = _toolStackEditor.CreateVisualElement();
            root.Add(toolEditorElement);
        }

        private void CreateHeader(VisualElement root)
        {
            var header = new VisualElement();
            header.style.backgroundColor = new Color(0.2f, 0.2f, 0.2f);
            header.style.paddingTop = 10;
            header.style.paddingBottom = 10;
            header.style.paddingLeft = 15;
            header.style.paddingRight = 15;
            header.style.marginBottom = 5;

            var title = new Label("Tool System");
            title.style.fontSize = 16;
            title.style.unityFontStyleAndWeight = FontStyle.Bold;
            title.style.color = Color.white;
            header.Add(title);

            var description = new Label("Manage and organize your editor tools");
            description.style.fontSize = 12;
            description.style.color = new Color(0.8f, 0.8f, 0.8f);
            header.Add(description);

            root.Add(header);
        }
    }
}
