using Cysharp.Threading.Tasks;
using DG.Tweening;
using DG.Tweening.Core;

namespace VladislavTsurikov.DOTweenUtility.Runtime
{
    public static class UniTaskExtensions
    {
        public static async UniTask AsyncWaitForCompletion(this Tween tween)
        {
            if (!tween.active)
            {
                if (Debugger.logPriority > 0)
                {
                    Debugger.LogInvalidTween(tween);
                }

                return;
            }

            while (tween.active && !tween.IsComplete())
            {
                await UniTask.Yield();
            }
        }
    }
}
