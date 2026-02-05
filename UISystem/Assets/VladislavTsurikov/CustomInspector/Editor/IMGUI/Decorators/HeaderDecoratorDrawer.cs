#if UNITY_EDITOR
using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.CustomInspector.Editor.Core;

namespace VladislavTsurikov.CustomInspector.Editor.IMGUI.Decorators
{
    public sealed class HeaderDecoratorDrawerMatcher : DecoratorDrawerMatcher<IMGUIDecoratorDrawer>
    {
        public override bool CanDraw(Attribute attribute) => attribute is HeaderAttribute;
        public override Type DrawerType => typeof(HeaderDecoratorDrawer);
    }

    public sealed class HeaderDecoratorDrawer : IMGUIDecoratorDrawer
    {
        private string _text;

        public override void Draw(Rect rect, FieldInfo field, object target)
        {
            if (Attribute is HeaderAttribute headerAttribute)
            {
                _text = headerAttribute.header;
            }

            EditorGUI.LabelField(rect, _text, EditorStyles.boldLabel);
        }

        public override float GetHeight(FieldInfo field, object target)
        {
            return EditorGUIUtility.singleLineHeight;
        }
    }
}
#endif
