#if UNITY_EDITOR
using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.Nody.Runtime.AdvancedNodeStack;
using VladislavTsurikov.Nody.Runtime.Core;
using VladislavTsurikov.CustomInspector.Editor.Core;
using VladislavTsurikov.CustomInspector.Editor.IMGUI;
using VladislavTsurikov.ReflectionUtility.Runtime;

namespace VladislavTsurikov.IMGUIUtility.Editor.ElementStack.ReorderableList
{
    public sealed class ReorderableListStackEditorFieldDrawerMatcher : FieldDrawerMatcher<IMGUIFieldDrawer>
    {
        public override bool CanDraw(FieldInfo field) =>
            field.FieldType.TryGetGenericArgument(typeof(AdvancedNodeStack<>)) != null;

        public override Type DrawerType => typeof(ReorderableListStackEditorFieldDrawer);
    }

    public class ReorderableListStackEditorFieldDrawer : IMGUIFieldDrawer
    {
        public override object Draw(Rect rect, GUIContent label, FieldInfo field, object target, object value)
        {
            if (value == null)
            {
                return null;
            }

            var collectionEditor = CreateEditor(value, field.FieldType, label);

            if (collectionEditor == null)
            {
                return null;
            }

            var listRect = new Rect(rect.x, rect.y + EditorGUIUtility.singleLineHeight, rect.width,
                rect.height - EditorGUIUtility.singleLineHeight);

            MethodInfo onGuiMethod = collectionEditor.GetType().GetMethod(
                "OnGUI",
                new[] { typeof(Rect) }
            );

            onGuiMethod?.Invoke(collectionEditor, new object[] { listRect });

            return value;
        }

        public override bool ShouldCreateInstanceIfNull() => true;


        public override float GetFieldsHeight(object target, FieldInfo field, object value)
        {
            if (target == null)
            {
                return EditorGUIUtility.singleLineHeight;
            }

            var collectionEditor = CreateEditor(target, target.GetType(), GUIContent.none);
            if (collectionEditor == null ||
                collectionEditor.GetType().GetMethod("GetElementStackHeight") is not { } getHeightMethod)
            {
                Debug.LogWarning("GetFieldsHeight: Missing collectionEditor or GetElementStackHeight method.");
                return EditorGUIUtility.singleLineHeight * 5;
            }

            return (float)getHeightMethod.Invoke(collectionEditor, null);
        }

        private static object CreateEditor(object value, Type fieldType, GUIContent label)
        {
            Type componentType = fieldType.TryGetGenericArgument(typeof(NodeStack<>));

            if (componentType == null)
            {
                Debug.LogError($"CreateEditor: Unable to determine componentType for {fieldType.FullName}.");
                return null;
            }

            Type editorType = typeof(ReorderableListStackEditor<,>).MakeGenericType(
                componentType,
                typeof(ReorderableListComponentEditor)
            );

            return Activator.CreateInstance(
                editorType,
                label,
                value,
                true
            );
        }
    }

}
#endif
