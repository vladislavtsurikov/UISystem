using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VladislavTsurikov.ReflectionUtility;

namespace VladislavTsurikov.ActionFlow.Runtime.Actions.Log
{
    [Name("Debug/Log Text")]
    public class LogTextAction : Action
    {
        private readonly string _message = "My message";

        public override string Name => $"Log: {_message}";

        protected override UniTask<bool> Run(CancellationToken token)
        {
            Debug.Log(_message);
            return UniTask.FromResult(true);
        }
    }
}
