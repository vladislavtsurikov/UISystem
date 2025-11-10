using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VladislavTsurikov.ActionFlow.Runtime.Variables;
using VladislavTsurikov.ReflectionUtility;

namespace VladislavTsurikov.ActionFlow.Runtime.Actions.GameObject
{
    [Name("GameObject/Instantiate")]
    public class GameObjectInstantiate : GameObjectAction
    {
        [SerializeField]
        private UnityEngine.GameObject _parent;

        [SerializeField]
        private Transform _point;

        [SerializeField]
        private UnityEngine.GameObject _prefab;

        [SerializeField]
        private GameObjectVariable _storage;

        public override string Name => $"Instantiate {_prefab}";

        protected override UniTask<bool> Run(CancellationToken token)
        {
            if (_prefab == null)
            {
                return UniTask.FromResult(true);
            }

            Vector3 position = _point != null ? _point.position : Vector3.zero;
            Quaternion rotation = _point != null ? _point.rotation : Quaternion.identity;

            UnityEngine.GameObject instance = Object.Instantiate(_prefab, position, rotation);

            if (_parent != null)
            {
                instance.transform.SetParent(_parent.transform);
            }

            if (_storage != null)
            {
                _storage.Value = instance;
            }

            return UniTask.FromResult(true);
        }
    }
}
