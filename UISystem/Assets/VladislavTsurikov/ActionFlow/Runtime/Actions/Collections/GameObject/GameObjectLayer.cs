using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VladislavTsurikov.ReflectionUtility;

namespace VladislavTsurikov.ActionFlow.Runtime.Actions.GameObject
{
    [Name("GameObject/Set Layer")]
    public class GameObjectLayer : GameObjectAction
    {
        private readonly bool _childrenToo = false;

        [SerializeField]
        private LayerMask _layer;

        public override string Name => string.Format(
            "Change Layer to {0} on {1} {2}",
            _layer,
            GameObject,
            _childrenToo ? "and children" : string.Empty
        );

        protected override UniTask<bool> Run(CancellationToken token)
        {
            if (GameObject == null)
            {
                return UniTask.FromResult(true);
            }

            GameObject.layer = (int)Mathf.Log(_layer.value, 2);

            if (_childrenToo)
            {
                Transform[] children = GameObject.GetComponentsInChildren<Transform>();
                foreach (Transform child in children)
                {
                    child.gameObject.layer = (int)Mathf.Log(_layer.value, 2);
                }
            }

            return UniTask.FromResult(true);
        }
    }
}
