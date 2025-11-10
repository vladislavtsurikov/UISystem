#if UNITY_EDITOR
using VladislavTsurikov.AutoDefines.Editor;

namespace VladislavTsurikov.AddressableLoaderSystem.Editor.Core
{
    public sealed class AddressablesDefineRule : PackageDefineRule
    {
        protected override string GetPackageId()
        {
            return "com.unity.addressables"; 
        }

        protected override string GetDefineToApplySymbol()
        {
            return "ADDRESSABLE_LOADER_SYSTEM_ADDRESSABLES";
        }
    }
}
#endif
