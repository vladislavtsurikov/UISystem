using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VladislavTsurikov.ReflectionUtility;

namespace VladislavTsurikov.ActionFlow.Runtime.Actions.Log
{
    [Name("Debug/Comment")]
    public class Comment : Action
    {
        [SerializeField]
        public string _comment;

        public override string Name => $"Comment: {_comment}";

        protected override UniTask<bool> Run(CancellationToken token) => UniTask.FromResult(true);
    }
}
