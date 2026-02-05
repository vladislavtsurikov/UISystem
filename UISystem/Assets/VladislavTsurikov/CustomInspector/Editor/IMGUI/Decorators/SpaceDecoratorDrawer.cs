#if UNITY_EDITOR
using System;
using System.Reflection;
using UnityEngine;
using VladislavTsurikov.CustomInspector.Editor.Core;

namespace VladislavTsurikov.CustomInspector.Editor.IMGUI.Decorators
{
    public sealed class SpaceDecoratorDrawerMatcher : DecoratorDrawerMatcher<IMGUIDecoratorDrawer>
    {
        public override bool CanDraw(Attribute attribute) => attribute is SpaceAttribute;
        public override Type DrawerType => typeof(SpaceDecoratorDrawer);
    }

    public sealed class SpaceDecoratorDrawer : IMGUIDecoratorDrawer
    {
        private float _height;

        public override void Draw(Rect rect, FieldInfo field, object target)
        {
            if (Attribute is SpaceAttribute spaceAttribute)
            {
                _height = spaceAttribute.height;
            }
        }

        public override float GetHeight(FieldInfo field, object target)
        {
            if (Attribute is SpaceAttribute spaceAttribute)
            {
                return spaceAttribute.height;
            }

            return 8f;
        }
    }
}
#endif
