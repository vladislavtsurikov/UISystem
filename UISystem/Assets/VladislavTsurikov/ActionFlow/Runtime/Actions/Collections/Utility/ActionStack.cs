using System.Threading;
using Cysharp.Threading.Tasks;
using OdinSerializer;
using VladislavTsurikov.ReflectionUtility;

namespace VladislavTsurikov.ActionFlow.Runtime.Actions.Utility
{
    [Name("Utility/Action Stack")]
    public class ActionStack : Action
    {
        [OdinSerialize]
        public ActionCollection ActionCollection = new();

        protected override async UniTask<bool> Run(CancellationToken token)
        {
            await ActionCollection.Run(token);
            return true;
        }
    }
}
