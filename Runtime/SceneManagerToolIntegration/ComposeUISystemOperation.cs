#if UI_SYSTEM_ADDRESSABLE_LOADER_SYSTEM && UI_SYSTEM_SCENE_MANAGER_TOOL && UI_SYSTEM_ZENJECT
using System.Threading;
using Cysharp.Threading.Tasks;
using VladislavTsurikov.Nody.Runtime.Core;
using VladislavTsurikov.ReflectionUtility;
using VladislavTsurikov.SceneManagerTool.Runtime.SettingsSystem;
using VladislavTsurikov.SceneUtility.Runtime;
using Zenject;
using Action = VladislavTsurikov.ActionFlow.Runtime.Actions.Action;
using Single = VladislavTsurikov.SceneManagerTool.Runtime.SceneTypeSystem.Single;

namespace VladislavTsurikov.UISystem.Runtime.AddressableLoaderSystemIntegration
{
    [Name("UISystem/Compose UI")]
    [ParentRequired(typeof(AfterLoadOperationsSettings), typeof(Single))]
    public sealed class ComposeUISystemOperation : Action
    {
        private SceneReference _sceneReference = new();

        [Inject]
        private SceneUICompositionService _sceneUICompositionService;

        protected override void SetupComponent(object[] setupData = null)
        {
            Single single = ContextHierarchy.GetContext<Single>();
            if (single != null)
            {
                _sceneReference = single.SceneReference;
            }
        }

        protected override async UniTask<bool> Run(CancellationToken token)
        {
            await _sceneUICompositionService.RecomposeScene(_sceneReference?.SceneName, token);

            return true;
        }
    }
}
#endif
