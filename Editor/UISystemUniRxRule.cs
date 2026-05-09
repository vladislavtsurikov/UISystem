#if UNITY_EDITOR
using VladislavTsurikov.AutoDefines.Editor;

namespace VladislavTsurikov.UISystem.Editor
{
    public sealed class UISystemUniRxRule : TypeDefineRule
    {
        protected override string GetDefineToApplySymbol()
        {
            return "UI_SYSTEM_UNIRX";
        }

        public override string GetTypeFullName()
        {
            return "UniRx.Unit";
        }
    }
}
#endif
