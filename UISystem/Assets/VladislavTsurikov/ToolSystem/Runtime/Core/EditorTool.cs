using VladislavTsurikov.Nody.Runtime.Core;

namespace VladislavTsurikov.ToolSystem.Runtime.Core
{
    public abstract class EditorTool : Node
    {
        protected override void SetupComponent(object[] setupData = null)
        {
            OnSetupTool();
        }

        protected virtual void OnSetupTool()
        {
        }
    }
}
