using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace VladislavTsurikov.UIToolkitUtility.Editor.ElementStack.TabStack
{
    public class Tab : VisualElement
    {
        private const string LayoutPath =
            "Assets/VladislavTsurikov/UIToolkitUtility/Editor/ElementStack/TabStack/Tab.uxml";
        private const string StylePath =
            "Assets/VladislavTsurikov/UIToolkitUtility/Editor/ElementStack/TabStack/Tab.uss";
        private const string LayoutContainerName = "LayoutContainer";
        private const string LabelName = "TabLabel";
        private const string TabClassName = "tab-stack-editor__tab";
        private const string TabLabelClassName = "tab-stack-editor__tab-label";

        public event Action Clicked;

        public Tab()
        {
            AddToClassList(TabClassName);

            var layout = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(LayoutPath);
            if (layout != null)
            {
                TemplateContainer = layout.CloneTree();
                Add(TemplateContainer);
                LayoutContainer = TemplateContainer.Q<VisualElement>(LayoutContainerName) ?? TemplateContainer;
                TabLabel = LayoutContainer.Q<Label>(LabelName) ?? CreateFallbackLabel();
                TabLabel.AddToClassList(TabLabelClassName);
                if (TabLabel.parent == null)
                {
                    LayoutContainer.Add(TabLabel);
                }
            }
            else
            {
                LayoutContainer = new VisualElement { name = LayoutContainerName };
                TabLabel = CreateFallbackLabel();
                TabLabel.AddToClassList(TabLabelClassName);
                LayoutContainer.Add(TabLabel);
                Add(LayoutContainer);
            }

            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(StylePath);
            if (styleSheet != null)
            {
                styleSheets.Add(styleSheet);
            }

            this.AddManipulator(new Clickable(() => Clicked?.Invoke()));
        }

        public TemplateContainer TemplateContainer { get; }
        public VisualElement LayoutContainer { get; }
        public Label TabLabel { get; }

        public string Text
        {
            get => TabLabel.text;
            set => TabLabel.text = value;
        }

        public void SetLabelColor(Color color)
        {
            TabLabel.style.color = color;
        }

        public void SetBackgroundColor(Color color)
        {
            style.backgroundColor = color;
        }

        private static Label CreateFallbackLabel()
        {
            var label = new Label { name = LabelName };
            return label;
        }
    }
}
