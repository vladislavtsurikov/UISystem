using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using VladislavTsurikov.ComponentStack.Runtime.Core;
using VladislavTsurikov.SceneUtility.Runtime;

namespace VladislavTsurikov.SceneManagerTool.Runtime.SettingsSystem.OperationSystem
{
    public class Operation : Component
    {
        public virtual async UniTask DoOperation()
        {
            await UniTask.CompletedTask;
        }

        public virtual List<SceneReference> GetSceneReferences() => new();
    }
}
