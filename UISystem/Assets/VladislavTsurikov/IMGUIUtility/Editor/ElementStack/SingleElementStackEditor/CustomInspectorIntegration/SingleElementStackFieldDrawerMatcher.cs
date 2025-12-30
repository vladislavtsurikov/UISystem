#if UNITY_EDITOR
using System;
using Assemblies.VladislavTsurikov.ComponentStack.Runtime.SingleElementStack;
using VladislavTsurikov.CustomInspector.Editor.Core;
using VladislavTsurikov.CustomInspector.Editor.IMGUI;

namespace VladislavTsurikov.IMGUIUtility.Editor.ElementStack.SingleElementStackEditor.CustomInspectorIntegration
{
    public sealed class SingleElementStackFieldDrawerMatcher : FieldDrawerMatcher<IMGUIFieldDrawer>
    {
        public override bool CanDraw(Type fieldType) =>
            fieldType.IsGenericType && fieldType.GetGenericTypeDefinition() == typeof(SingleElementStack<>);

        public override Type DrawerType => typeof(SingleElementStackFieldDrawer);
    }
}
#endif
