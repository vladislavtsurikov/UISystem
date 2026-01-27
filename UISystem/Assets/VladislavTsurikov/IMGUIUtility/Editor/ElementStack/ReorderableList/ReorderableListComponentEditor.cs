#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.Nody.Editor.Core;
using VladislavTsurikov.Nody.Runtime.Core;
using VladislavTsurikov.CustomInspector.Editor.IMGUI;

namespace VladislavTsurikov.IMGUIUtility.Editor.ElementStack.ReorderableList
{
    public class ReorderableListComponentEditor : ElementEditor
    {
        protected readonly IMGUIInspectorFieldsDrawer _fieldsRenderer = new(
            new List<Type> { typeof(Node), typeof(Element) }
        );

        public virtual void OnGUI(Rect rect, int index)
        {
            if (Target == null)
            {
                return;
            }

            EditorGUI.BeginChangeCheck();
            _fieldsRenderer.DrawFields(Target, rect, index);
            if (EditorGUI.EndChangeCheck())
            {
                MarkTargetDirty();
            }
        }

        public virtual float GetElementHeight(int index)
        {
            if (Target == null)
            {
                return EditorGUIUtility.singleLineHeight;
            }

            return _fieldsRenderer.GetFieldsHeight(Target, index);
        }

    }
}
#endif
