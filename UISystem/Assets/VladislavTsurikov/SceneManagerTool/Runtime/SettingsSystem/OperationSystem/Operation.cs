using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using VladislavTsurikov.Nody.Runtime.Core;
using VladislavTsurikov.SceneUtility.Runtime;

namespace VladislavTsurikov.SceneManagerTool.Runtime.SettingsSystem.OperationSystem
{
    public class Operation : Node
    {
        public virtual async UniTask DoOperation()
        {
            await UniTask.CompletedTask;
        }

        public virtual List<SceneReference> GetSceneReferences() => new();
    }
}
