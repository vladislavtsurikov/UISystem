using System;
using Cysharp.Threading.Tasks;
using Zenject;

namespace VladislavTsurikov.UISystem.Runtime.Core
{
    public static class UIHandlerUtility
    {
        public static async UniTask EnsureHandlersReady()
        {
            if (UIHandlerManager.CurrentAddFilterTask.Status == UniTaskStatus.Pending)
            {
                await UIHandlerManager.CurrentAddFilterTask;
            }
        }

        public static T FindHandler<T>() where T : UIHandler => FindHandler<T>(null);

        public static T FindHandler<T>(Type parentType) where T : UIHandler
        {
            try
            {
                var id = UIHandlerBindingId.FromParentType(parentType);
                return ProjectContext.Instance.Container.ResolveId<T>(id);
            }
            catch
            {
                return null;
            }
        }
    }
}
