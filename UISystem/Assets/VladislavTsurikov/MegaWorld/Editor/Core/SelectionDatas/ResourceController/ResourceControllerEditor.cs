#if UNITY_EDITOR
using VladislavTsurikov.ComponentStack.Runtime.Core;

namespace VladislavTsurikov.MegaWorld.Editor.Core.SelectionDatas.ResourceController
{
    public abstract class ResourceControllerEditor : Component
    {
        public abstract void OnGUI(Runtime.Core.SelectionDatas.Group.Group group);
        public abstract bool HasSyncError(Runtime.Core.SelectionDatas.Group.Group group);
    }
}
#endif
