#if UNITY_EDITOR
using VladislavTsurikov.AutoDefines.Editor;

namespace VladislavTsurikov.ActionFlow.Editor
{
    public sealed class ActionFlowZenjectRule : TypeDefineRule
    {
        protected override string GetDefineToApplySymbol() { return "ACTIONFLOW_ZENJECT"; }
        public override string GetTypeFullName() => "Zenject.DiContainer";
    }
}
#endif
