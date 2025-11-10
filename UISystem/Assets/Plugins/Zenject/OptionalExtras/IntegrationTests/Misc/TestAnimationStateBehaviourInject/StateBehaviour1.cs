using UnityEngine;

namespace Zenject.Tests.TestAnimationStateBehaviourInject
{
    public class StateBehaviour1 : StateMachineBehaviour
    {
        public static int OnStateEnterCalls;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) =>
            OnStateEnterCalls++;
    }
}
