using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace VladislavTsurikov.AddressableLoaderSystem.Editor.Core.Create.ResourceLoaderDescriptorElements
{
    public abstract class ResourceLoaderDescriptorElement : VisualElement
    {
        private BehaviorAttributesElement _behaviorsElement;
        private TextField _classNameField;

        protected VisualElement _formContainer;
        protected ResourceLoaderTemplate Template;


        public event Action OnClassNameChanged;

        public ResourceLoaderDescriptorElement(ResourceLoaderTemplate template)
        {
            Template = template;

            _formContainer = new VisualElement();
            _formContainer.style.marginTop = 10;
            _formContainer.style.flexDirection = FlexDirection.Column;
            Add(_formContainer);

            RefreshForm();
        }

        protected abstract void OnGUI();

        public void RefreshForm()
        {
            _formContainer.Clear();

            _classNameField = new TextField("Class Name");
            _classNameField.value = Template.ClassName;
            _classNameField.RegisterValueChangedCallback(evt =>
            {
                Template.ClassName = evt.newValue;
                OnClassNameChanged?.Invoke();
            });
            _formContainer.Add(_classNameField);

            OnGUI();

            var behaviorsBlock = CreateSectionBlock("Behavior Attributes");
            _behaviorsElement = new BehaviorAttributesElement(Template.Behaviors);
            behaviorsBlock.Add(_behaviorsElement);
            _formContainer.Add(behaviorsBlock);
        }

        public VisualElement CreateSectionBlock(string title)
        {
            var box = new VisualElement();
            box.style.marginTop = 8;
            box.style.marginBottom = 10;
            box.style.paddingTop = 6;
            box.style.paddingBottom = 6;
            box.style.paddingLeft = 8;
            box.style.paddingRight = 8;
            box.style.backgroundColor = new Color(0.18f, 0.18f, 0.18f, 0.7f);
            box.style.borderTopWidth = 1;
            box.style.borderBottomWidth = 1;
            box.style.borderTopColor = new Color(0.3f, 0.3f, 0.3f);
            box.style.borderBottomColor = new Color(0.3f, 0.3f, 0.3f);
            box.style.flexDirection = FlexDirection.Column;

            var header = new Label(title);
            header.style.unityFontStyleAndWeight = FontStyle.Bold;
            header.style.marginBottom = 4;
            box.Add(header);

            return box;
        }
    }
}
