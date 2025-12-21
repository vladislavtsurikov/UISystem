#if UNITY_EDITOR
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using VladislavTsurikov.Core.Editor;

namespace VladislavTsurikov.AddressableLoaderSystem.Editor.Core
{
    [ElementEditor(typeof(PrefabResourceLoaderTemplate))]
    public class PrefabResourceLoaderTemplateElement : ResourceLoaderTemplateElement
    {
        private ObjectField _prefabObjectField;

        public PrefabResourceLoaderTemplateElement(ResourceLoaderTemplate template) : base(template)
        {
        }

        protected override void OnGUI()
        {
            var prefabTemplate = (PrefabResourceLoaderTemplate)Template;

            VisualElement addressBlock = CreateSectionBlock("Prefab");

            _prefabObjectField = new ObjectField("Prefab")
            {
                value = prefabTemplate.FieldData.Asset,
                objectType = typeof(UnityEngine.Object),
                allowSceneObjects = false
            };
            _prefabObjectField.RegisterValueChangedCallback(evt =>
            {
                prefabTemplate.FieldData.Asset = evt.newValue;
                prefabTemplate.UpdatePrefabAddressFromFieldData();
            });

            addressBlock.Add(_prefabObjectField);
            _formContainer.Add(addressBlock);
        }
    }
}
#endif
