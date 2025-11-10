#if UNITY_ANDROID || UNITY_IOS
using System.Threading;
using Cysharp.Threading.Tasks;
using Neutral.Utility.Runtime;
using UniRx;
using UnityEngine;

namespace UIBackSystem.Runtime
{
    public static class UIBackInputListener
    {
        private static readonly AsyncTaskQueue _backQueue = new();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Init()
        {
            Observable
                .EveryUpdate()
                .Where(_ => Input.GetKeyDown(KeyCode.Escape))
                .Subscribe(_ => _backQueue.Enqueue(HandleBackButton));
        }

        private static async UniTask HandleBackButton()
        {
            await UIBackStack.PopAndHide(CancellationToken.None);
        }
    }
}
#endif
