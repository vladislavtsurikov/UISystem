using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace VladislavTsurikov.UISystem.Runtime.Core
{
    public static class UINavigator
    {
        public static async UniTask Show<T>(CancellationToken ct = default)
            where T : UIHandler =>
            await Handle<T>(null, true, ct);

        public static async UniTask Show<T, TParent>(CancellationToken ct = default)
            where T : UIHandler
            where TParent : UIHandler =>
            await Handle<T>(typeof(TParent), true, ct);

        public static async UniTask Hide<T>(CancellationToken ct = default)
            where T : UIHandler =>
            await Handle<T>(null, false, ct);

        public static async UniTask Hide<T, TParent>(CancellationToken ct = default)
            where T : UIHandler
            where TParent : UIHandler =>
            await Handle<T>(typeof(TParent), false, ct);

        private static async UniTask Handle<T>(Type parentType, bool isShow, CancellationToken ct)
            where T : UIHandler
        {
            await UIHandlerUtility.EnsureHandlersReady();

            T handler = parentType == null
                ? UIHandlerUtility.FindHandler<T>()
                : UIHandlerUtility.FindHandler<T>(parentType);

            if (handler == null)
            {
                Debug.LogError(
                    $"[UINavigator] Cannot {(isShow ? "Show" : "Hide")}. UIHandler of type {typeof(T).Name}" +
                    (parentType != null ? $" with parent {parentType.Name}" : "") + " not found.");
                return;
            }

            if (isShow)
            {
                await handler.Show(ct);
            }
            else
            {
                await handler.Hide(ct);
            }
        }
    }
}
