using System.Threading;
using Cysharp.Threading.Tasks;
using VladislavTsurikov.ActionFlow.Runtime.Actions;
using VladislavTsurikov.ComponentStack.Runtime.AdvancedComponentStack;

namespace VladislavTsurikov.ActionFlow.Runtime
{
    public class ActionCollection : ComponentStackSupportSameType<Action>
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
