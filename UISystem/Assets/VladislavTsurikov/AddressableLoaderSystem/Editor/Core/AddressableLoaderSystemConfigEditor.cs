using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using VladislavTsurikov.AddressableLoaderSystem.Editor.Core.Warning;
using VladislavTsurikov.AddressableLoaderSystem.Runtime.Core;

namespace VladislavTsurikov.AddressableLoaderSystem.Editor.Core
{
    [CustomEditor(typeof(AddressableLoaderSystemConfig))]
    public class AddressableLoaderSystemConfigEditor : UnityEditor.Editor
    {
        private AddressableLoaderSystemConfig _config;
        private Button _refreshButton;

        private VisualElement _root;
        private VisualElement _warningContainer;

        private void OnEnable()
        {
            _config = (AddressableLoaderSystemConfig)target;
            _config.Refresh();

            _warningContainer = new VisualElement { style = { marginBottom = 6 } };

            UpdateWarningSection();
        }

        public override VisualElement CreateInspectorGUI()
        {
            _root = new VisualElement { style = { paddingTop = 6, paddingLeft = 6, paddingRight = 6 } };

            AddressableLoaderSystemConfig config = (AddressableLoaderSystemConfig)target;

            _refreshButton = new Button(() =>
            {
                config.Refresh();
                UpdateWarningSection();
            })
            { text = "Refresh caches" };

            _root.Add(_refreshButton);
            _root.Add(_warningContainer);

            InspectorElement.FillDefaultInspector(_root, serializedObject, this);

            UpdateWarningSection();

            return _root;
        }

        private void UpdateWarningSection()
        {
            _warningContainer.Clear();

            List<AddressableLoaderValidator.ValidationResult> validationResults =
                AddressableLoaderValidator.ValidateAll();

            List<string> invalidLoaders = validationResults
                .Where(r => r.Issues != null && r.Issues.Count > 0 && r.LoaderType != null)
                .Select(r => r.LoaderType.Name)
                .Distinct()
                .ToList();

            if (invalidLoaders.Count == 0)
            {
                return;
            }

            MissingAutoLoadWarningElement warningElement = new MissingAutoLoadWarningElement(invalidLoaders);
            _warningContainer.Add(warningElement);
        }
    }
}
