using System.Threading;
using Cysharp.Threading.Tasks;
using VladislavTsurikov.ReflectionUtility;

namespace VladislavTsurikov.ActionFlow.Runtime.Actions.GameObject
{
    [Name("GameObject/Set Active")]
    public class GameObjectSetActive : GameObjectAction
    {
        public bool ActiveGameObject;

        public override string Name => $"Set Active {GameObject} to {ActiveGameObject}";

        protected override UniTask<bool> Run(CancellationToken token)
        {
            GameObject.SetActive(ActiveGameObject);
            return UniTask.FromResult(true);
        }
    }
}
