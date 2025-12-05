using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using VladislavTsurikov.ReflectionUtility;

namespace VladislavTsurikov.ActionFlow.Runtime.Actions.GameObject
{
    [Name("GameObject/Add Component")]
    public class GameObjectAddComponent : GameObjectAction
    {
        public string ComponentType;

        public override string Name => $"Add Component {ComponentType} to {GameObject}";

        protected override UniTask<bool> Run(CancellationToken token)
        {
            var componentType = Type.GetType(ComponentType);
            if (componentType != null)
            {
                GameObject.AddComponent(componentType);
            }

            return UniTask.FromResult(true);
        }
    }
}
