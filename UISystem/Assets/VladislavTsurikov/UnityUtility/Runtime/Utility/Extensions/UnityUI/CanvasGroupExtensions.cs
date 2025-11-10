using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace VladislavTsurikov.UnityUtility.Runtime
{
    public static class CanvasGroupExtensions
    {
        public static async UniTask Fade(this CanvasGroup group, float to, float seconds, bool setBlocksRaycasts = true)
        {
            if (!group || !group.gameObject.activeInHierarchy)
            {
                return;
            }

            if (setBlocksRaycasts)
            {
                group.blocksRaycasts = true;
            }

            if (group.alpha == to)
            {
                return;
            }

            await Lerp(group.alpha, to, seconds, t =>
            {
                if (group)
                {
                    group.alpha = t;
                }

                if (setBlocksRaycasts)
                {
                    group.blocksRaycasts = group.alpha > 0;
                }
            });
        }

        private static async UniTask Lerp(float start, float end, float seconds, Action<float> callback)
        {
            var waitForSeconds = seconds;
            var timeLeft = seconds;

            while (timeLeft >= 0)
            {
                var t = Mathf.InverseLerp(waitForSeconds, 0, timeLeft);
                callback?.Invoke(Mathf.Lerp(start, end, t));

                timeLeft -= Time.deltaTime;

                await UniTask.Yield();
            }

            callback?.Invoke(end);
        }
    }
}
