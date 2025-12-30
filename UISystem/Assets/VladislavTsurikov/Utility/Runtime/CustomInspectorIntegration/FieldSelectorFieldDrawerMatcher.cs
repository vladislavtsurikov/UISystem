#if UNITY_EDITOR
using System;
using VladislavTsurikov.CustomInspector.Editor.Core;
using VladislavTsurikov.CustomInspector.Editor.IMGUI;
using VladislavTsurikov.Utility.Runtime.CustomInspectorIntegration;

namespace QuestsSystem.IntegrationActionFlow.Pointer
{
    public sealed class FieldSelectorFieldDrawerMatcher : FieldDrawerMatcher<IMGUIFieldDrawer>
    {
        public override bool CanDraw(Type fieldType) => typeof(FieldSelector).IsAssignableFrom(fieldType);
        public override Type DrawerType => typeof(FieldSelectorFieldDrawer);
    }
}
#endif
