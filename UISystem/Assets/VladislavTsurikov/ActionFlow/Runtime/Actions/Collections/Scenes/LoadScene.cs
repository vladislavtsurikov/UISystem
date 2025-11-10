using System.Threading;
using Cysharp.Threading.Tasks;
using VladislavTsurikov.ReflectionUtility;
using VladislavTsurikov.SceneUtility.Runtime;

namespace VladislavTsurikov.ActionFlow.Runtime.Actions.Scenes
{
    [Name("Scene/Load Scene")]
    public class LoadScene : Action
    {
        public SceneReference SceneReference = new();

        public override string Name
        {
            get
            {
                if (!SceneReference.IsValid())
                {
                    return "Load Scene [Set Scene]";
                }

                return $"Load Scene [{SceneReference.Scene.name}]";
            }
        }

        protected override async UniTask<bool> Run(CancellationToken token)
        {
            await SceneReference.LoadScene();
            return true;
        }
    }
}
