#if UNITY_EDITOR
using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.UIElements;
using VladislavTsurikov.CustomInspector.Editor.Core;

namespace VladislavTsurikov.CustomInspector.Editor.UIToolkit.Decorators
{
    public sealed class SpaceDecoratorDrawerMatcher : DecoratorDrawerMatcher<UIToolkitDecoratorDrawer>
    {
        public override bool CanDraw(Attribute attribute) => attribute is SpaceAttribute;
        public override Type DrawerType => typeof(SpaceDecoratorDrawer);
    }

    public sealed class SpaceDecoratorDrawer : UIToolkitDecoratorDrawer
    {
        public override VisualElement CreateElement(FieldInfo field, object target)
        {
            float height = 8f;

            if (Attribute is SpaceAttribute spaceAttribute)
            {
                height = spaceAttribute.height;
            }

            var space = new VisualElement();
            space.style.height = height;

            return space;
        }
    }
}
#endif

