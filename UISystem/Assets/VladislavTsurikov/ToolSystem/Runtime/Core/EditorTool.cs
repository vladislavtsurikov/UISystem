using VladislavTsurikov.ComponentStack.Runtime.Core;

namespace VladislavTsurikov.ToolSystem.Runtime.Core
{
    public abstract class EditorTool : Component
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
