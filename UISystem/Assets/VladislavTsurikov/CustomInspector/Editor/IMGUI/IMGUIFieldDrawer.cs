#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.CustomInspector.Editor.Core;

namespace VladislavTsurikov.CustomInspector.Editor.IMGUI
{
    public abstract class IMGUIFieldDrawer : FieldDrawer
    {
        public abstract object Draw(Rect rect, GUIContent label, Type fieldType, object value);

        public virtual float GetFieldsHeight(object target) =>
            EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
    }
}
#endif
