using System.Threading;
using Cysharp.Threading.Tasks;
using VladislavTsurikov.ActionFlow.Runtime.Actions;
using VladislavTsurikov.Nody.Runtime.AdvancedNodeStack;

namespace VladislavTsurikov.ActionFlow.Runtime
{
    public class ActionCollection : NodeStackSupportSameType<Action>
    {
        public async UniTask<bool> Run(CancellationToken token = default)
        {
            foreach (Action action in ElementList)
            {
                token.ThrowIfCancellationRequested();
                var isActionCompleted = await action.RunAction(token);

                if (!isActionCompleted)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
