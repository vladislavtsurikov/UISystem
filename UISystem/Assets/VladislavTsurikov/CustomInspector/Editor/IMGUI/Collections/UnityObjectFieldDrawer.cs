#if UNITY_EDITOR
using System;
using System.Collections;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;
using VladislavTsurikov.CustomInspector.Editor.Core;

namespace VladislavTsurikov.CustomInspector.Editor.IMGUI
{
    public sealed class UnityObjectFieldDrawerMatcher : FieldDrawerMatcher<IMGUIFieldDrawer>
    {
        public override bool CanDraw(FieldInfo field) => typeof(Object).IsAssignableFrom(field.FieldType);
        public override Type DrawerType => typeof(UnityObjectFieldDrawer);
    }

    public class UnityObjectFieldDrawer : IMGUIFieldDrawer
    {
        public override object Draw(Rect rect, GUIContent label, FieldInfo field, object target, object value)
        {
            Type objectType = field.FieldType;

            // Handle case when called from ListFieldDrawer with collection FieldInfo
            if (objectType.IsGenericType && typeof(IList).IsAssignableFrom(objectType))
            {
                objectType = objectType.GetGenericArguments()[0];
            }

            return EditorGUI.ObjectField(rect, label, (Object)value, objectType, true);
        }

        public override bool ShouldCreateInstanceIfNull() => false;
    }
}
#endif


