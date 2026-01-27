using System.Threading;
using Cysharp.Threading.Tasks;
using VladislavTsurikov.Nody.Runtime.Core;

namespace VladislavTsurikov.ActionFlow.Runtime.Actions
{
    public abstract class Action : Node
    {
        public async UniTask<bool> RunAction(CancellationToken token)
        {
            if (Active)
            {
                return await Run(token);
            }

            return true;
        }

        protected virtual UniTask<bool> Run(CancellationToken token)
        {
            return UniTask.FromResult(true);
        }
    }
}
