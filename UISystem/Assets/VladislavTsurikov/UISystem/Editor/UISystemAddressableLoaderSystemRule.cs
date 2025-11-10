#if UNITY_EDITOR
using VladislavTsurikov.AutoDefines.Editor;

namespace VladislavTsurikov.UISystem.Editor
{
    public sealed class UISystemAddressableLoaderSystemRule : TypeDefineRule
    {
        protected override string GetDefineToApplySymbol() => "UI_SYSTEM_ADDRESSABLE_LOADER_SYSTEM";
        public override string GetTypeFullName() => "VladislavTsurikov.AddressableLoaderSystem.Runtime.Core.ResourceLoaderManager";
    }
}
#endif
