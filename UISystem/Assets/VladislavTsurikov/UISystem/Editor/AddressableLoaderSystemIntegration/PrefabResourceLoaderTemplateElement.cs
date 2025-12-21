#if UNITY_EDITOR
using UnityEngine.UIElements;
using VladislavTsurikov.Core.Editor;

namespace VladislavTsurikov.AddressableLoaderSystem.Editor.Core
{
    [ElementEditor(typeof(PrefabResourceLoaderTemplate))]
    public class PrefabResourceLoaderTemplateElement : ResourceLoaderTemplateElement
    {
        private TextField _prefabAddressField;

        public PrefabResourceLoaderTemplateElement(ResourceLoaderTemplate template) : base(template)
        {
        }

        protected override void OnGUI()
        {
            var prefabTemplate = (PrefabResourceLoaderTemplate)Template;

            VisualElement addressBlock = CreateSectionBlock("Prefab");

            _prefabAddressField = new TextField("Address")
            {
                value = prefabTemplate.PrefabAddress
            };
            _prefabAddressField.RegisterValueChangedCallback(evt =>
            {
                prefabTemplate.PrefabAddress = evt.newValue;
            });

            addressBlock.Add(_prefabAddressField);
            _formContainer.Add(addressBlock);
        }
    }
}
#endif
