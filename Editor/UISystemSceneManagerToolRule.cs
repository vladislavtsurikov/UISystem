#if UNITY_EDITOR
using VladislavTsurikov.AutoDefines.Editor;

namespace VladislavTsurikov.UISystem.Editor
{
    public sealed class UISystemSceneManagerToolRule : TypeDefineRule
    {
        protected override string GetDefineToApplySymbol()
        {
            return "UI_SYSTEM_SCENE_MANAGER_TOOL";
        }

        public override string GetTypeFullName()
        {
            return "VladislavTsurikov.SceneManagerTool.Runtime.SettingsSystem.AfterLoadOperationsSettings";
        }
    }
}
#endif
