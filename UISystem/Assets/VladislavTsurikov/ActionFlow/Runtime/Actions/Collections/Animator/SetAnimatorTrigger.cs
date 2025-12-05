using System.Threading;
using Cysharp.Threading.Tasks;
using VladislavTsurikov.ReflectionUtility;

namespace VladislavTsurikov.ActionFlow.Runtime.Actions.Animator
{
    [Name("Animator/Set Trigger")]
    public class SetAnimatorTrigger : ActionAnimator
    {
        private readonly string _parameter = "My Parameter";

        public override string Name => $"Set Animator Trigger {_parameter}";

        protected override UniTask<bool> Run(CancellationToken token)
        {
            Animator.SetTrigger(_parameter);
            return UniTask.FromResult(true);
        }
    }
}
