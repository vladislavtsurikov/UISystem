using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using VladislavTsurikov.DOTweenUtility.Runtime;
using VladislavTsurikov.ReflectionUtility;

namespace VladislavTsurikov.ActionFlow.Runtime.Actions.Animator
{
    [Name("Animator/Change Integer")]
    public class ChangeAnimatorInteger : ActionAnimator
    {
        private readonly string _parameter = "My Parameter";

        private readonly int _targetValue = 1;

        private readonly Transition _transition = new();

        public override string Name => $"Change Animator Integer {_parameter}";

        protected override async UniTask<bool> Run(CancellationToken token)
        {
            Tweener tween = DOTween.To(
                () => Animator.GetInteger(_parameter),
                value => Animator.SetInteger(_parameter, value),
                _targetValue,
                _transition.Duration
            ).ApplyTransition(_transition);

            await tween.AsyncWaitForCompletion(_transition);
            return true;
        }
    }
}
