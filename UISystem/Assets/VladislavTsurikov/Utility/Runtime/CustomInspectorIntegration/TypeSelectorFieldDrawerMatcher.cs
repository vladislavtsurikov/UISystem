#if UNITY_EDITOR
using System;
using VladislavTsurikov.CustomInspector.Editor.Core;
using VladislavTsurikov.CustomInspector.Editor.IMGUI;
using VladislavTsurikov.Utility.Runtime.CustomInspectorIntegration;

namespace VladislavTsurikov.ReflectionUtility.Runtime.CustomInspectorIntegration
{
    public sealed class TypeSelectorFieldDrawerMatcher : FieldDrawerMatcher<IMGUIFieldDrawer>
    {
        public override bool CanDraw(Type fieldType) =>
            fieldType.IsGenericType && fieldType.GetGenericTypeDefinition() == typeof(TypeSelector<>);

        public override Type DrawerType => typeof(TypeSelectorFieldDrawer);
    }
}
#endif
