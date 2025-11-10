#if UI_SYSTEM_ADDRESSABLE_LOADER_SYSTEM
#if UI_SYSTEM_ZENJECT
using System.Threading;
using Cysharp.Threading.Tasks;
using VladislavTsurikov.AddressableLoaderSystem.Runtime.Core;
using VladislavTsurikov.UISystem.Runtime.Core;
using Zenject;
using UIBindingId = VladislavTsurikov.UISystem.Runtime.UnityUIIntegration.Utility.UIBindingId;

namespace VladislavTsurikov.UISystem.Tests.Runtime.MissionTogglePresenters
{
    [SceneFilter("TestScene_1")]
    [ParentUIHandler(typeof(UIMissionsMainWindowHandler))]
    public class ProgressMissionTogglePresenter : UIMissionTogglePresenter
    {
        public ProgressMissionTogglePresenter(DiContainer container)
            : base(container)
        {
        }

        protected override bool UnlockedTab => true;
        protected override int NotificationCount => 10;

        protected override UIMissionToggleView ResolveView() =>
            _container.ResolveId<UIMissionToggleView>(
                UIBindingId.FromTypeAndIndex(typeof(UIMissionsMainWindowHandler), "Progress"));

        protected override async UniTask OnToggleClicked(CancellationToken cancellationToken) =>
            await UINavigator.Show<ProgressMissionsWindowHandler, UIMissionsMainWindowHandler>(cancellationToken);
    }
}

#endif

#endif
