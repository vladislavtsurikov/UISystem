#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEngine.UIElements;
using VladislavTsurikov.CustomInspector.Editor.UIToolkit;
using VladislavTsurikov.Nody.Editor.Core;
using VladislavTsurikov.Nody.Runtime.Core;

namespace VladislavTsurikov.IMGUIUtility.Editor.ElementStack.UIToolkitReorderableList
{
    public class UIToolkitReorderableListComponentEditor : ElementEditor
    {
        protected readonly UIToolkitInspectorFieldsDrawer _fieldsRenderer = new(
            new List<Type> { typeof(Node), typeof(Element) }
        );

        private VisualElement _contentContainer;

        public virtual VisualElement CreateGUI()
        {
            if (Target == null)
            {
                return new Label("Target is null");
            }

            _contentContainer = _fieldsRenderer.CreateFieldsContainer(Target);

            // Register value change callbacks for all fields to mark dirty
            RegisterFieldCallbacks(_contentContainer);

            return _contentContainer;
        }

        public virtual void UpdateGUI()
        {
            if (_contentContainer != null && Target != null)
            {
                _contentContainer.Clear();
                var newContent = _fieldsRenderer.CreateFieldsContainer(Target);
                RegisterFieldCallbacks(newContent);

                foreach (var child in newContent.Children())
                {
                    _contentContainer.Add(child);
                }
            }
        }

        private void RegisterFieldCallbacks(VisualElement container)
        {
            // Listen for any value changes in the fields
            container.RegisterCallback<ChangeEvent<object>>(evt =>
            {
                Target?.MarkDirty();
            });
        }
    }
}
#endif
