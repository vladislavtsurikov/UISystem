#if UNITY_EDITOR
using VladislavTsurikov.AutoDefines.Editor;

namespace VladislavTsurikov.EntityDataAction.Shared.Editor
{
    public sealed class MPUIKitRule : TypeDefineRule
    {
        protected override string GetDefineToApplySymbol() => "ENTITY_DATA_ACTION_MPUIKIT";
        public override string GetTypeFullName() => "MPUIKIT.MPImage";
    }
}
#endif
