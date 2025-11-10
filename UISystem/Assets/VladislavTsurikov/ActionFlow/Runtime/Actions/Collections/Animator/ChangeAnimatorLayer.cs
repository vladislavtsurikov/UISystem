using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using VladislavTsurikov.DOTweenUtility.Runtime;
using VladislavTsurikov.ReflectionUtility;

namespace VladislavTsurikov.ActionFlow.Runtime.Actions.Animator
{
    [Name("Animator/Change Layer")]
    public class ChangeAnimatorLayer : ActionAnimator
    {
        private readonly int _layerIndex = 1;

        private readonly float _targetWeight = 1f;

        private readonly Transition _transition = new();

        public override string Name => $"Change Layer {_layerIndex} Weight";

        protected override async UniTask<bool> Run(CancellationToken token)
        {
            Tweener tween = Animator
                .DOSetLayerWeight(_layerIndex, _targetWeight, _transition.Duration)
                .ApplyTransition(_transition);

            await tween.AsyncWaitForCompletion(_transition);
            return true;
        }
    }
}
