#if UNITY_EDITOR
using VladislavTsurikov.AutoDefines.Editor;

namespace VladislavTsurikov.ActionFlow.Editor
{
    public sealed class AddressableLoaderSystemZenjectRule : TypeDefineRule
    {
        protected override string GetDefineToApplySymbol() { return "ADDRESSABLE_LOADER_SYSTEM_ZENJECT"; }
        public override string GetTypeFullName() => "Zenject.DiContainer";
    }
}
#endif
