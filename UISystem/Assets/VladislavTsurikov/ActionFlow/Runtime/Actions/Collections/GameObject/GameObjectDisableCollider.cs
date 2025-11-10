using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VladislavTsurikov.ReflectionUtility;

namespace VladislavTsurikov.ActionFlow.Runtime.Actions.GameObject
{
    [Name("GameObject/Disable Collider")]
    public class GameObjectDisableCollider : GameObjectAction
    {
        public override string Name => $"Disable Collider on {GameObject}";

        protected override UniTask<bool> Run(CancellationToken token)
        {
            Collider collider = GameObject.GetComponent<Collider>();
            if (collider != null)
            {
                collider.enabled = false;
            }

            return UniTask.FromResult(true);
        }
    }
}
