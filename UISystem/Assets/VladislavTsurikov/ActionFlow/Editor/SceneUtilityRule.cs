#if UNITY_EDITOR
using VladislavTsurikov.AutoDefines.Editor;

namespace VladislavTsurikov.ActionFlow.Editor
{
    public sealed class SceneUtilityRule : TypeDefineRule
    {
        protected override string GetDefineToApplySymbol() => "SCENE_REFERENCE_UTILITY";
        public override string GetTypeFullName() => "VladislavTsurikov.SceneUtility.Runtime.SceneReference";
    }
}
#endif
