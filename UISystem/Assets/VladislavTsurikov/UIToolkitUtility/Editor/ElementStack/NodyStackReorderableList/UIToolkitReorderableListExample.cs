#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using VladislavTsurikov.AttributeUtility.Runtime;
using VladislavTsurikov.Nody.Runtime.AdvancedNodeStack;
using VladislavTsurikov.Nody.Runtime.Core;

namespace VladislavTsurikov.IMGUIUtility.Editor.ElementStack.UIToolkitReorderableList
{
    /// <summary>
    /// Example window demonstrating UIToolkit ReorderableList for ComponentStack
    /// </summary>
    public class UIToolkitReorderableListExample : EditorWindow
    {
        [Serializable]
        [Name("Transform Component")]
        public class TransformComponent : Node
        {
            public Vector3 Position;
            public Vector3 Rotation;
            public Vector3 Scale = Vector3.one;

            protected override void OnCreate()
            {
                base.OnCreate();
                Name = "Transform";
            }
        }

        [Serializable]
        [Name("Renderer Component")]
        public class RendererComponent : Node
        {
            public Color Color = Color.white;
            public Material Material;
            public bool CastShadows = true;

            protected override void OnCreate()
            {
                base.OnCreate();
                Name = "Renderer";
            }
        }

        [Serializable]
        [Name("Physics Component")]
        public class PhysicsComponent : Node
        {
            public float Mass = 1f;
            public float Drag = 0.5f;
            public bool UseGravity = true;

            protected override void OnCreate()
            {
                base.OnCreate();
                Name = "Physics";
            }
        }

        [Serializable]
        public class ExampleComponentStack : NodeStackSupportSameType<Node>
        {
            protected override void OnSetup()
            {
                base.OnSetup();
            }
        }

        private ExampleComponentStack _componentStack;
        private UIToolkitReorderableListStackEditor<Component, UIToolkitReorderableListComponentEditor> _stackEditor;

        [MenuItem("Tools/Vladislav Tsurikov/UIToolkit ComponentStack ReorderableList Example")]
        public static void ShowWindow()
        {
            var window = GetWindow<UIToolkitReorderableListExample>();
            window.titleContent = new GUIContent("UIToolkit ComponentStack Example");
            window.minSize = new Vector2(400, 600);
        }

        public void CreateGUI()
        {
            InitializeComponentStack();

            var root = rootVisualElement;
            root.style.paddingLeft = 10;
            root.style.paddingRight = 10;
            root.style.paddingTop = 10;
            root.style.paddingBottom = 10;

            // Title
            var title = new Label("UIToolkit ComponentStack ReorderableList");
            title.style.fontSize = 16;
            title.style.unityFontStyleAndWeight = FontStyle.Bold;
            title.style.marginBottom = 10;
            root.Add(title);

            // Description
            var description = new Label(
                "This example demonstrates a UIToolkit-based ReorderableList for ComponentStack.\n" +
                "Features: drag and drop reordering, add/remove components, context menu, " +
                "active toggle, foldout, rename support.");
            description.style.whiteSpace = WhiteSpace.Normal;
            description.style.marginBottom = 10;
            description.style.fontSize = 11;
            root.Add(description);

            // Create the reorderable list editor
            _stackEditor = new UIToolkitReorderableListStackEditor<Component, UIToolkitReorderableListComponentEditor>(
                new GUIContent("Component Stack"),
                _componentStack,
                true)
            {
                DisplayHeaderText = true,
                DisplayPlusButton = true,
                DuplicateSupport = true,
                RenameSupport = true,
                ShowActiveToggle = true,
                RemoveSupport = true,
                ReorderSupport = true
            };

            var stackElement = _stackEditor.GetVisualElement();
            root.Add(stackElement);

            // Refresh button
            var refreshButton = new Button(() =>
            {
                _stackEditor.RefreshEditors();
                root.Clear();
                CreateGUI();
            });
            refreshButton.text = "Refresh";
            refreshButton.style.marginTop = 10;
            root.Add(refreshButton);
        }

        private void InitializeComponentStack()
        {
            _componentStack = new ExampleComponentStack();
            _componentStack.Setup(true);

            // Add some default components if empty
            if (_componentStack.ElementList.Count == 0)
            {
                _componentStack.CreateNode(typeof(TransformComponent));
                _componentStack.CreateNode(typeof(RendererComponent));
                _componentStack.CreateNode(typeof(PhysicsComponent));
            }
        }

        private void OnDisable()
        {
            _componentStack?.OnDisable();
        }
    }
}
#endif
