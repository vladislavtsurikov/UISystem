using System;
using Cysharp.Threading.Tasks;
using VladislavTsurikov.Core.Runtime.DependencyInjection;

namespace VladislavTsurikov.UISystem.Runtime.Core
{
    public static class UIPresenterResolver
    {
        public static async UniTask EnsurePresentersReady()
        {
            if (UIPresenterManager.CurrentAddFilterTask.Status == UniTaskStatus.Pending)
            {
                await UIPresenterManager.CurrentAddFilterTask;
            }
        }

        public static T FindPresenter<T>() where T : UIPresenter
        {
            return FindPresenter<T>(null);
        }

        public static T FindPresenter<T>(Type parentType, string instanceKey) where T : UIPresenter
        {
            UIPresenterKey key = UIPresenterKey.FromParentType(parentType, instanceKey);
            if (TryResolve(key, out T presenter))
            {
                return presenter;
            }

            return null;
        }

        public static T FindPresenter<T>(Type parentType) where T : UIPresenter
        {
            return FindPresenter<T>(parentType, null);
        }

        internal static bool TryResolve<T>(UIPresenterKey key, out T presenter) where T : UIPresenter
        {
            if (Dependencies.TryResolveId(typeof(T), key.Id, out object instance) &&
                instance is T typedPresenter)
            {
                presenter = typedPresenter;
                return true;
            }

            presenter = null;
            return false;
        }
    }
}
