using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VladislavTsurikov.ReflectionUtility;

namespace VladislavTsurikov.ActionFlow.Runtime.Actions.GameObject
{
    [Name("GameObject/Disable Component")]
    public class GameObjectDisableComponent : GameObjectAction
    {
        public string ComponentType;

        public override string Name => $"Disable Component {ComponentType} on {GameObject}";

        protected override UniTask<bool> Run(CancellationToken token)
        {
            var componentType = Type.GetType(ComponentType);
            if (componentType != null)
            {
                var component = (MonoBehaviour)GameObject.GetComponent(componentType);
                if (component != null)
                {
                    component.enabled = false;
                }
            }

            return UniTask.FromResult(true);
        }
    }
}
