using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using VladislavTsurikov.ComponentStack.Runtime.AdvancedComponentStack;
using VladislavTsurikov.ReflectionUtility;
using VladislavTsurikov.SceneManagerTool.Runtime.SettingsSystem.OperationSystem;
using VladislavTsurikov.SceneUtility.Runtime;

namespace VladislavTsurikov.SceneManagerTool.Runtime.SettingsSystem
{
    [Name("After Unload")]
    public class AfterUnloadOperationsSettings : SettingsComponent
    {
        public ComponentStackSupportSameType<Operation> OperationStack = new();

        public async UniTask DoOperations()
        {
            foreach (Operation sceneOperation in OperationStack.ElementList)
            {
                await sceneOperation.DoOperation();
            }
        }

        public override List<SceneReference> GetSceneReferences()
        {
            var sceneReferences =
                new List<SceneReference>();

            foreach (Operation operation in OperationStack.ElementList)
            {
                sceneReferences.AddRange(operation.GetSceneReferences());
            }

            return sceneReferences;
        }
    }
}
