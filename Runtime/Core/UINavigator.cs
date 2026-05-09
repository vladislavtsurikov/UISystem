using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace VladislavTsurikov.UISystem.Runtime.Core
{
    public static class UINavigator
    {
        public static async UniTask Show<T>(CancellationToken ct = default)
            where T : UIPresenter
        {
            await Handle<T>(true, ct);
        }

        public static async UniTask Hide<T>(CancellationToken ct = default)
            where T : UIPresenter
        {
            await Handle<T>(false, ct);
        }

        private static async UniTask Handle<T>(bool isShow, CancellationToken ct)
            where T : UIPresenter
        {
            Type parentType = ResolveParentType<T>();

            T presenter = parentType == null
                ? UIPresenterResolver.FindPresenter<T>()
                : UIPresenterResolver.FindPresenter<T>(parentType);

            if (presenter == null)
            {
                Debug.LogError(
                    $"[UINavigator] Cannot {(isShow ? "Show" : "Hide")}. UIPresenter of type {typeof(T).Name}" +
                    (parentType != null ? $" with parent {parentType.Name}" : "") + " not found.");
                return;
            }

            if (isShow)
            {
                await presenter.Show(ct);
            }
            else
            {
                await presenter.Hide(ct);
            }
        }

        private static Type ResolveParentType<T>() where T : UIPresenter
        {
            UIParentAttribute attribute =
                (UIParentAttribute)Attribute.GetCustomAttribute(typeof(T), typeof(UIParentAttribute));
            return attribute?.ParentType;
        }
    }
}
