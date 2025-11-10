using DG.Tweening;
using UnityEngine;

namespace VladislavTsurikov.DOTweenUtility.Runtime
{
    public static class AnimatorExtensions
    {
        public static Tweener DOFloat(this Animator animator, string parameter, float endValue, float duration)
        {
            var parameterHash = Animator.StringToHash(parameter);
            return DOTween.To(
                () => animator.GetFloat(parameterHash),
                value => animator.SetFloat(parameterHash, value),
                endValue,
                duration
            );
        }

        public static Tweener DOInt(this Animator animator, string parameter, int endValue, float duration)
        {
            var parameterHash = Animator.StringToHash(parameter);
            return DOTween.To(
                () => animator.GetInteger(parameterHash),
                value => animator.SetInteger(parameterHash, value),
                endValue,
                duration
            );
        }

        public static Tweener DOBool(this Animator animator, string parameter, bool endValue, float duration)
        {
            var parameterHash = Animator.StringToHash(parameter);
            return DOTween.To(
                () => animator.GetBool(parameterHash) ? 1f : 0f,
                value => animator.SetBool(parameterHash, value > 0.5f),
                endValue ? 1f : 0f,
                duration
            );
        }

        public static void DOTrigger(this Animator animator, string parameter)
        {
            var parameterHash = Animator.StringToHash(parameter);
            animator.SetTrigger(parameterHash);
        }

        public static Tweener DOSetLayerWeight(this Animator animator, int layerIndex, float endValue, float duration)
        {
            if (layerIndex < 0 || layerIndex >= animator.layerCount)
            {
                Debug.LogError(
                    $"Layer index {layerIndex} is out of range. The animator has {animator.layerCount} layers.");
                return null;
            }

            return DOTween.To(
                () => animator.GetLayerWeight(layerIndex),
                value => animator.SetLayerWeight(layerIndex, value),
                endValue,
                duration
            );
        }
    }
}
