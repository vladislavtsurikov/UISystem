#if UNITY_EDITOR
using System;
using VladislavTsurikov.ComponentStack.Runtime.AdvancedComponentStack;
using VladislavTsurikov.CustomInspector.Editor.Core;
using VladislavTsurikov.CustomInspector.Editor.IMGUI;
using VladislavTsurikov.ReflectionUtility.Runtime;

namespace VladislavTsurikov.IMGUIUtility.Editor.ElementStack.ReorderableList
{
    public sealed class ReorderableListStackEditorFieldDrawerMatcher : FieldDrawerMatcher<IMGUIFieldDrawer>
    {
        public override bool CanDraw(Type fieldType) =>
            fieldType.TryGetGenericArgument(typeof(AdvancedComponentStack<>)) != null;

        public override Type DrawerType => typeof(ReorderableListStackEditorFieldDrawer);
    }
}
#endif
