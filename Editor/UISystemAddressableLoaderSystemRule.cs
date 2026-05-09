#if UNITY_EDITOR
using VladislavTsurikov.AutoDefines.Editor;

namespace VladislavTsurikov.UISystem.Editor
{
    public sealed class UISystemAddressableLoaderSystemRule : TypeDefineRule
    {
        protected override string GetDefineToApplySymbol()
        {
            return "UI_SYSTEM_ADDRESSABLE_LOADER_SYSTEM";
        }

        public override string GetTypeFullName()
        {
            return "VladislavTsurikov.AddressableLoaderSystem.Runtime.Core.ResourceLoaderManager";
        }
    }
}
#endif
