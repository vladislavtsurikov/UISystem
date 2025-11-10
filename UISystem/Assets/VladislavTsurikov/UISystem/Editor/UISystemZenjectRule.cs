#if UNITY_EDITOR
using VladislavTsurikov.AutoDefines.Editor;

namespace VladislavTsurikov.UISystem.Editor.AutoDefines
{
    public sealed class UISystemZenjectRule : TypeDefineRule
    {
        protected override string GetDefineToApplySymbol() => "UI_SYSTEM_ZENJECT";
        public override string GetTypeFullName() => "Zenject.DiContainer";
    }
}
#endif
