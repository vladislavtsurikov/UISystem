using UnityEngine;

namespace VladislavTsurikov.ActionFlow.Runtime.Actions.Animator
{
    public abstract class ActionAnimator : Action
    {
        [SerializeField]
        private UnityEngine.Animator _animator;

        protected UnityEngine.Animator Animator => _animator;
    }
}
