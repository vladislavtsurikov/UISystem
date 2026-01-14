#if UNITY_EDITOR
using System;
using UnityEngine;
using VladislavTsurikov.CustomInspector.Editor.Core;

namespace VladislavTsurikov.CustomInspector.Editor.IMGUI.Decorators
{
    public sealed class SpaceDecoratorDrawerMatcher : DecoratorDrawerMatcher<IMGUIDecoratorDrawer>
    {
        public override bool CanProcess(Attribute attribute) => attribute is SpaceAttribute;
        public override Type DrawerType => typeof(SpaceDecoratorDrawer);
    }

    public sealed class SpaceDecoratorDrawer : IMGUIDecoratorDrawer
    {
        private float _height;

        public override void Draw(Rect rect)
        {
            // Space doesn't draw anything, it just adds vertical space
            if (Attribute is SpaceAttribute spaceAttribute)
            {
                _height = spaceAttribute.height;
            }
        }

        public override float GetHeight()
        {
            if (Attribute is SpaceAttribute spaceAttribute)
            {
                return spaceAttribute.height;
            }

            return 8f; // Default Unity space height
        }
    }
}
#endif
