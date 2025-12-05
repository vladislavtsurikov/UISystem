#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.ComponentStack.Editor.Core;
using VladislavTsurikov.ComponentStack.Runtime.Core;
using VladislavTsurikov.CustomInspector.Editor.IMGUI;
using Runtime_Core_Component = VladislavTsurikov.ComponentStack.Runtime.Core.Component;

namespace VladislavTsurikov.IMGUIUtility.Editor.ElementStack.ReorderableList
{
    public class ReorderableListComponentEditor : ElementEditor
    {
        private readonly IMGUIInspectorFieldsDrawer _fieldsRenderer = new(
            new List<Type> { typeof(Runtime_Core_Component), typeof(Element) }
        );

        public virtual void OnGUI(Rect rect, int index)
        {
            if (Target == null)
            {
                return;
            }

            _fieldsRenderer.DrawFields(Target, rect);
        }

        public virtual float GetElementHeight(int index)
        {
            if (Target == null)
            {
                return EditorGUIUtility.singleLineHeight;
            }

            return _fieldsRenderer.GetFieldsHeight(Target);
        }
    }
}
#endif
