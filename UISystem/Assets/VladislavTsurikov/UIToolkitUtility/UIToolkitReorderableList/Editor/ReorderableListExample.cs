#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using VladislavTsurikov.UIToolkitReorderableList.Runtime;

namespace VladislavTsurikov.UIToolkitReorderableList.Editor
{
    /// <summary>
    /// Example window demonstrating ReorderableList usage
    /// </summary>
    public class ReorderableListExample : EditorWindow
    {
        [Serializable]
        public class ExampleData
        {
            public string Name;
            public int Value;
            public Color Color;
        }

        private List<string> _simpleList;
        private List<ExampleData> _complexList;
        private List<GameObject> _objectList;

        [MenuItem("Tools/Vladislav Tsurikov/UIToolkit ReorderableList Example")]
        public static void ShowWindow()
        {
            var window = GetWindow<ReorderableListExample>();
            window.titleContent = new GUIContent("ReorderableList Example");
            window.minSize = new Vector2(400, 600);
        }

        public void CreateGUI()
        {
            InitializeData();

            var root = rootVisualElement;
            root.style.paddingLeft = 10;
            root.style.paddingRight = 10;
            root.style.paddingTop = 10;
            root.style.paddingBottom = 10;

            // Title
            var title = new Label("UIToolkit ReorderableList Examples");
            title.style.fontSize = 16;
            title.style.unityFontStyleAndWeight = FontStyle.Bold;
            title.style.marginBottom = 10;
            root.Add(title);

            // Example 1: Simple String List
            root.Add(CreateSimpleListExample());

            // Example 2: Complex Object List
            root.Add(CreateComplexListExample());

            // Example 3: Unity Object List
            root.Add(CreateUnityObjectListExample());
        }

        private void InitializeData()
        {
            _simpleList = new List<string> { "Apple", "Banana", "Cherry", "Date", "Elderberry" };

            _complexList = new List<ExampleData>
            {
                new ExampleData { Name = "Item 1", Value = 100, Color = Color.red },
                new ExampleData { Name = "Item 2", Value = 200, Color = Color.green },
                new ExampleData { Name = "Item 3", Value = 300, Color = Color.blue }
            };

            _objectList = new List<GameObject>();
        }

        private VisualElement CreateSimpleListExample()
        {
            var container = new VisualElement();
            container.style.marginBottom = 20;

            var label = new Label("Simple String List");
            label.style.unityFontStyleAndWeight = FontStyle.Bold;
            label.style.marginBottom = 5;
            container.Add(label);

            var reorderableList = new ReorderableList
            {
                HeaderTitle = "Fruits",
                ShowHeader = true,
                ShowFooter = true,
                Reorderable = true
            };

            reorderableList.MakeItem = () => new TextField();

            reorderableList.BindItem = (element, index) =>
            {
                if (element is TextField textField && index < _simpleList.Count)
                {
                    textField.value = _simpleList[index];
                    textField.RegisterValueChangedCallback(evt => _simpleList[index] = evt.newValue);
                }
            };

            reorderableList.OnAdd = list =>
            {
                _simpleList.Add("New Item");
                reorderableList.Refresh();
            };

            reorderableList.ItemsSource = _simpleList;
            reorderableList.style.minHeight = 150;

            container.Add(reorderableList);

            return container;
        }

        private VisualElement CreateComplexListExample()
        {
            var container = new VisualElement();
            container.style.marginBottom = 20;

            var label = new Label("Complex Object List");
            label.style.unityFontStyleAndWeight = FontStyle.Bold;
            label.style.marginBottom = 5;
            container.Add(label);

            var reorderableList = new ReorderableList
            {
                HeaderTitle = "Example Data",
                ShowHeader = true,
                ShowFooter = true,
                Reorderable = true
            };

            reorderableList.MakeItem = () =>
            {
                var itemContainer = new VisualElement();
                itemContainer.style.paddingTop = 4;
                itemContainer.style.paddingBottom = 4;

                itemContainer.Add(new TextField("Name"));
                itemContainer.Add(new IntegerField("Value"));
                itemContainer.Add(new ColorField("Color"));

                return itemContainer;
            };

            reorderableList.BindItem = (element, index) =>
            {
                if (index >= _complexList.Count) return;

                var data = _complexList[index];

                var nameField = element[0] as TextField;
                var valueField = element[1] as IntegerField;
                var colorField = element[2] as ColorField;

                if (nameField != null)
                {
                    nameField.value = data.Name;
                    nameField.RegisterValueChangedCallback(evt => data.Name = evt.newValue);
                }

                if (valueField != null)
                {
                    valueField.value = data.Value;
                    valueField.RegisterValueChangedCallback(evt => data.Value = evt.newValue);
                }

                if (colorField != null)
                {
                    colorField.value = data.Color;
                    colorField.RegisterValueChangedCallback(evt => data.Color = evt.newValue);
                }
            };

            reorderableList.OnAdd = list =>
            {
                _complexList.Add(new ExampleData { Name = "New Item", Value = 0, Color = Color.white });
                reorderableList.Refresh();
            };

            reorderableList.ItemsSource = _complexList;
            reorderableList.style.minHeight = 200;

            container.Add(reorderableList);

            return container;
        }

        private VisualElement CreateUnityObjectListExample()
        {
            var container = new VisualElement();
            container.style.marginBottom = 20;

            var label = new Label("Unity Object List");
            label.style.unityFontStyleAndWeight = FontStyle.Bold;
            label.style.marginBottom = 5;
            container.Add(label);

            var reorderableList = new ReorderableList
            {
                HeaderTitle = "GameObjects",
                ShowHeader = true,
                ShowFooter = true,
                Reorderable = true
            };

            reorderableList.MakeItem = () =>
            {
                return new UnityEditor.UIElements.ObjectField
                {
                    objectType = typeof(GameObject)
                };
            };

            reorderableList.BindItem = (element, index) =>
            {
                if (element is UnityEditor.UIElements.ObjectField objectField && index < _objectList.Count)
                {
                    objectField.value = _objectList[index];
                    objectField.RegisterValueChangedCallback(evt =>
                    {
                        _objectList[index] = evt.newValue as GameObject;
                    });
                }
            };

            reorderableList.OnAdd = list =>
            {
                _objectList.Add(null);
                reorderableList.Refresh();
            };

            reorderableList.ItemsSource = _objectList;
            reorderableList.style.minHeight = 150;

            container.Add(reorderableList);

            return container;
        }
    }
}
#endif
