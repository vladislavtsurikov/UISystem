using System.Threading;
using Cysharp.Threading.Tasks;
using VladislavTsurikov.ComponentStack.Runtime.Core;

namespace VladislavTsurikov.ActionFlow.Runtime.Actions
{
    public abstract class Action : Component
    {
        internal async UniTask<bool> RunAction(CancellationToken token)
        {
            if (Active)
            {
                return await Run(token);
            }

            return true;
        }

        protected abstract UniTask<bool> Run(CancellationToken token);
    }
}
