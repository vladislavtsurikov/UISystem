#if UI_SYSTEM_ADDRESSABLE_LOADER_SYSTEM
#if UI_SYSTEM_UNIRX
#if UI_SYSTEM_ZENJECT
using System.Threading;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using VladislavTsurikov.AddressableLoaderSystem.Runtime.Core;
using VladislavTsurikov.UISystem.Runtime.Core;
using VladislavTsurikov.UISystem.Runtime.UnityUIIntegration;
using Zenject;

namespace VladislavTsurikov.UISystem.Tests.Runtime
{
    [SceneFilter("TestScene_1")]
    [ParentUIHandler(typeof(UIMissionsMainWindowHandler))]
    public class ProgressMissionsWindowHandler : UnityUIHandler
    {
        private readonly MissionViewLoader _missionViewLoader;
        private bool _spawnedOnce;

        private MissionWindowView _view;

        public ProgressMissionsWindowHandler(
            DiContainer container, GeneralMissionLoader generalMissionLoader,
            MissionViewLoader missionViewLoader) : base(container, generalMissionLoader) =>
            _missionViewLoader = missionViewLoader;

        protected override Transform GetSpawnParentTransform()
        {
            var mainWindowHandler = (UIMissionsMainWindowHandler)Parent;
            return mainWindowHandler.View.MissionSpawnRect;
        }

        protected override string GetParentName() => "ProgressMissionsWindow";


        protected override async UniTask AfterShowUIHandler(CancellationToken ct, CompositeDisposable disposables)
        {
            if (_view == null)
            {
                _view = GetUIComponent<MissionWindowView>("MissionWindowView");
            }

            if (_spawnedOnce)
            {
                return;
            }

            _spawnedOnce = true;

            for (var i = 0; i < 10; i++)
            {
                await SpawnChildPrefab(_missionViewLoader, _view.MissionSpawnRect, true, ct);
            }
        }
    }
}

#endif

#endif

#endif
