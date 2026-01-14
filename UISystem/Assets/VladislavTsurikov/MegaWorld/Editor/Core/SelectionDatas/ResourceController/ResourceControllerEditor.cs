#if UNITY_EDITOR
using VladislavTsurikov.Nody.Runtime.Core;

namespace VladislavTsurikov.MegaWorld.Editor.Core.SelectionDatas.ResourceController
{
    public abstract class ResourceControllerEditor : Node
    {
        public abstract void OnGUI(Runtime.Core.SelectionDatas.Group.Group group);
        public abstract bool HasSyncError(Runtime.Core.SelectionDatas.Group.Group group);
    }
}
#endif
