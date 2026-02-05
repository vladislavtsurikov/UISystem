#if UNITY_EDITOR
using System.Reflection;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.CustomInspector.Editor.Core;

namespace VladislavTsurikov.CustomInspector.Editor.IMGUI
{
    public abstract class IMGUIFieldDrawer : FieldDrawer
    {
        public abstract object Draw(Rect rect, GUIContent label, FieldInfo field, object target, object value);

        public virtual float GetFieldsHeight(object target, FieldInfo field, object value) =>
            EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
    }
}
#endif
