using System;
using DG.Tweening;
using UnityEngine;

namespace VladislavTsurikov.DOTweenUtility.Runtime
{
    [Serializable]
    public class Transition
    {
        [SerializeField]
        private float _duration;

        [SerializeField]
        private Ease _ease = Ease.InOutQuad;

        [SerializeField]
        private UpdateType _time = UpdateType.Normal;

        [SerializeField]
        private bool _waitToComplete = true;


        public Transition()
        {
        }

        public Transition(UpdateType time) => _time = time;

        public Transition(float duration, Ease ease, bool waitToComplete)
        {
            _duration = duration;
            _ease = ease;
            _waitToComplete = waitToComplete;
        }

        public float Duration => _duration;

        public Ease EaseType => _ease;

        public UpdateType Time => _time;

        public bool WaitToComplete => _waitToComplete;

        public Tweener ApplyTweenSettings(Tweener tweener)
        {
            tweener.SetEase(_ease).SetUpdate(_time);

            return tweener;
        }
    }
}
