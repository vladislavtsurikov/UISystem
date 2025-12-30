#if UNITY_EDITOR
using System;
using VladislavTsurikov.CustomInspector.Editor.Core;
using VladislavTsurikov.CustomInspector.Editor.IMGUI;
using VladislavTsurikov.SceneUtility.Runtime;

namespace VladislavTsurikov.SceneUtility.Editor.Integration.CustomInspector
{
    public sealed class SceneReferenceFieldDrawerMatcher : FieldDrawerMatcher<IMGUIFieldDrawer>
    {
        public override bool CanDraw(Type fieldType) => fieldType == typeof(SceneReference);
        public override Type DrawerType => typeof(SceneReferenceFieldDrawer);
    }
}
#endif
