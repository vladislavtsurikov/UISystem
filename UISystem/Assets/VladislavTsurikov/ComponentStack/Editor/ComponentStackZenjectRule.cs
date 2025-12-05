#if UNITY_EDITOR
using VladislavTsurikov.AutoDefines.Editor;

namespace VladislavTsurikov.ActionFlow.Editor
{
    public sealed class ComponentStackZenjectRule : TypeDefineRule
    {
        protected override string GetDefineToApplySymbol() { return "COMPONENT_STACK_ZENJECT"; }
        public override string GetTypeFullName() => "Zenject.DiContainer";
    }
}
#endif
