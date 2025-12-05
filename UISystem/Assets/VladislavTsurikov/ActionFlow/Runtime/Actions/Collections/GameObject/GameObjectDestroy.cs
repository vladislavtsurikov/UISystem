using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VladislavTsurikov.ReflectionUtility;

namespace VladislavTsurikov.ActionFlow.Runtime.Actions.GameObject
{
    [Name("GameObject/Destroy")]
    public class GameObjectDestroy : GameObjectAction
    {
        public override string Name => $"Destroy {GameObject}";

        protected override UniTask<bool> Run(CancellationToken token)
        {
            Object.Destroy(GameObject);
            return UniTask.FromResult(true);
        }
    }
}
