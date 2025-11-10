using Cysharp.Threading.Tasks;
using DG.Tweening;

namespace VladislavTsurikov.DOTweenUtility.Runtime
{
    public static class TransitionExtensions
    {
        public static Tweener ApplyTransition(this Tweener tweener, Transition transition) =>
            tweener
                .SetEase(transition.EaseType)
                .SetUpdate(transition.Time);

        public static async UniTask AsyncWaitForCompletion(this Tweener tweener, Transition transition)
        {
            if (transition.WaitToComplete)
            {
                await tweener.AsyncWaitForCompletion();
            }
        }
    }
}
