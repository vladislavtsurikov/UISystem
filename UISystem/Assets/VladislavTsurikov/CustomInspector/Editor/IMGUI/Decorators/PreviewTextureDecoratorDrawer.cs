#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.CustomInspector.Editor.Core;
using VladislavTsurikov.CustomInspector.Runtime;

namespace VladislavTsurikov.CustomInspector.Editor.IMGUI.Decorators
{
    public sealed class PreviewTextureDecoratorDrawerMatcher : DecoratorDrawerMatcher<IMGUIDecoratorDrawer>
    {
        public override bool CanDraw(Attribute attribute) => attribute is PreviewTextureAttribute;
        public override Type DrawerType => typeof(PreviewTextureDecoratorDrawer);
    }

    public sealed class PreviewTextureDecoratorDrawer : IMGUIDecoratorDrawer
    {
        public override void Draw(Rect rect)
        {
            if (Attribute is not PreviewTextureAttribute previewTextureAttribute)
            {
                return;
            }

            var context = InspectorContext.Current;
            if (context?.Field == null || context.Target == null)
            {
                return;
            }

            var texture = context.Field.GetValue(context.Target) as Texture;
            if (texture == null)
            {
                return;
            }

            var width = previewTextureAttribute.Width > 0 ? previewTextureAttribute.Width : rect.width;
            var previewRect = new Rect(rect.x, rect.y, width, previewTextureAttribute.Height);
            EditorGUI.DrawPreviewTexture(previewRect, texture);
        }

        public override float GetHeight()
        {
            return Attribute is PreviewTextureAttribute previewTextureAttribute
                ? previewTextureAttribute.Height
                : EditorGUIUtility.singleLineHeight;
        }
    }
}
#endif
