using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VladislavTsurikov.ActionFlow.Runtime.Variables;
using VladislavTsurikov.ReflectionUtility;

namespace VladislavTsurikov.ActionFlow.Runtime.Actions.Utility
{
    [Name("Utility/Destroy objects from List")]
    public class ClearListGameObjects : Action
    {
        [SerializeField]
        private ListGameObjects _listGameObjects;

        protected override UniTask<bool> Run(CancellationToken token)
        {
            if (_listGameObjects == null)
            {
                return UniTask.FromResult(true);
            }

            foreach (UnityEngine.GameObject go in _listGameObjects.Value)
            {
                if (go == null)
                {
                    continue;
                }

                Object.Destroy(go);
            }

            _listGameObjects.Value.Clear();
            return UniTask.FromResult(true);
        }
    }
}
