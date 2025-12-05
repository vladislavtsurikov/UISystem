using System.Threading;
using Cysharp.Threading.Tasks;
using VladislavTsurikov.ReflectionUtility;
using VladislavTsurikov.SceneUtility.Runtime;

namespace VladislavTsurikov.ActionFlow.Runtime.Actions.Scenes
{
    [Name("Scene/Unload Scene")]
    public class UnloadScene : Action
    {
        public SceneReference SceneReference = new();

        public override string Name
        {
            get
            {
                if (!SceneReference.IsValid())
                {
                    return "Unload Scene [Set Scene]";
                }

                return $"Unload Scene [{SceneReference.Scene.name}]";
            }
        }

        protected override async UniTask<bool> Run(CancellationToken token)
        {
            await SceneReference.UnloadScene();
            return true;
        }
    }
}
