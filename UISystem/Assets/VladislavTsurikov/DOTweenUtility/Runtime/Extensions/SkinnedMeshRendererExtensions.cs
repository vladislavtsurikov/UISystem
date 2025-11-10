using DG.Tweening;
using UnityEngine;

namespace VladislavTsurikov.DOTweenUtility.Runtime
{
    public static class SkinnedMeshRendererExtensions
    {
        public static Tweener DOBlendShapeWeight(this SkinnedMeshRenderer skinnedMeshRenderer, int blendShapeIndex,
            float endValue, float duration)
        {
            if (blendShapeIndex < 0 || blendShapeIndex >= skinnedMeshRenderer.sharedMesh.blendShapeCount)
            {
                Debug.LogError("BlendShape index is out of range.");
                return null;
            }

            return DOTween.To(
                () => skinnedMeshRenderer.GetBlendShapeWeight(blendShapeIndex),
                value => skinnedMeshRenderer.SetBlendShapeWeight(blendShapeIndex, value),
                endValue,
                duration
            );
        }

        public static Tweener DOBlendShapeWeight(this SkinnedMeshRenderer skinnedMeshRenderer, string blendShapeName,
            float endValue, float duration)
        {
            var blendShapeIndex = skinnedMeshRenderer.sharedMesh.GetBlendShapeIndex(blendShapeName);

            if (blendShapeIndex < 0)
            {
                Debug.LogError($"BlendShape '{blendShapeName}' not found.");
                return null;
            }

            return skinnedMeshRenderer.DOBlendShapeWeight(blendShapeIndex, endValue, duration);
        }
    }
}
