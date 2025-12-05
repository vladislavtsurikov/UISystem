using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using VladislavTsurikov.DOTweenUtility.Runtime;
using VladislavTsurikov.ReflectionUtility;

namespace VladislavTsurikov.ActionFlow.Runtime.Actions.Animator
{
    [Name("Animator/Change Float")]
    public class ChangeAnimatorFloat : ActionAnimator
    {
        private readonly string _parameter = "My Parameter";

        private readonly float _targetValue = 1f;

        private readonly Transition _transition = new();

        public override string Name => $"Change Animator Float {_parameter}";

        protected override async UniTask<bool> Run(CancellationToken token)
        {
            Tweener tween = DOTween.To(
                () => Animator.GetFloat(_parameter),
                value => Animator.SetFloat(_parameter, value),
                _targetValue,
                _transition.Duration
            ).ApplyTransition(_transition);

            token.Register(() => tween.Kill());

            await tween.AsyncWaitForCompletion(_transition).AttachExternalCancellation(token);
            return true;
        }
    }
}
