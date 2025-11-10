using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using VladislavTsurikov.DOTweenUtility.Runtime;
using VladislavTsurikov.ReflectionUtility;

namespace VladislavTsurikov.ActionFlow.Runtime.Actions.Animator
{
    [Name("Animator/Change Blend Shape")]
    public class ChangeBlendShape : ActionAnimator
    {
        private readonly string _blendShape = "Smile";

        private readonly float _targetValue = 1f;

        private readonly Transition _transition = new();

        [SerializeField]
        private SkinnedMeshRenderer _skinnedMesh;

        public override string Name => $"Change Blend-Shape {_blendShape}";

        protected override async UniTask<bool> Run(CancellationToken token)
        {
            var blendShapeIndex = _skinnedMesh.sharedMesh.GetBlendShapeIndex(_blendShape);
            if (blendShapeIndex == -1)
            {
                return true;
            }

            Tweener tween = _skinnedMesh
                .DOBlendShapeWeight(blendShapeIndex, _targetValue, _transition.Duration)
                .ApplyTransition(_transition);

            await tween.AsyncWaitForCompletion(_transition);
            return true;
        }
    }
}
