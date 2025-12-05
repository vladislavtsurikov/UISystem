#if UNITY_EDITOR
using VladislavTsurikov.AutoDefines.Editor;

namespace VladislavTsurikov.AddressableLoaderSystem.Editor
{
    public sealed class AddressableLoaderSystemVContainerRule : TypeDefineRule
    {
        protected override string GetDefineToApplySymbol() => "ADDRESSABLE_LOADER_SYSTEM_VCONTAINER";
        public override string GetTypeFullName() => "VContainer.IContainerBuilder";
    }
}
#endif
