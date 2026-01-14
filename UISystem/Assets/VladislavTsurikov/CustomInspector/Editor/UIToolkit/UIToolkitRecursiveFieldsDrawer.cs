#if UNITY_EDITOR
using System;
using System.Reflection;
using UnityEditor;
using UnityEngine.UIElements;
using VladislavTsurikov.CustomInspector.Editor.Core;

namespace VladislavTsurikov.CustomInspector.Editor.UIToolkit
{
    public class UIToolkitRecursiveFieldsDrawer : RecursiveFieldsDrawer
    {
        public VisualElement DrawRecursiveFields(
            object value,
            FieldInfo fieldInfo,
            Action<object, VisualElement> drawField)
        {
            var container = new VisualElement();

            var foldout = new Foldout
            {
                text = ObjectNames.NicifyVariableName(fieldInfo.Name),
                value = GetFoldoutState(value)
            };

            foldout.RegisterValueChangedCallback(evt =>
            {
                SetFoldoutState(value, evt.newValue);
            });

            var contentContainer = new VisualElement();
            contentContainer.style.paddingLeft = 15;

            if (foldout.value)
            {
                drawField(value, contentContainer);
            }

            foldout.RegisterValueChangedCallback(evt =>
            {
                contentContainer.Clear();
                if (evt.newValue)
                {
                    drawField(value, contentContainer);
                }
            });

            container.Add(foldout);
            container.Add(contentContainer);

            return container;
        }
    }
}
#endif
