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
    public class DailyMissionTogglePresenter : UIMissionTogglePresenter
    {
        public DailyMissionTogglePresenter(DiContainer container)
            : base(container)
        {
        }

        protected override bool UnlockedTab => true;
        protected override int NotificationCount => 6;

        protected override UIMissionToggleView ResolveView() =>
            _container.ResolveId<UIMissionToggleView>(
                UIBindingId.FromTypeAndIndex(typeof(UIMissionsMainWindowHandler), "Daily"));

        protected override async UniTask OnToggleClicked(CancellationToken cancellationToken) =>
            await UINavigator.Show<DailyMissionsWindowHandler, UIMissionsMainWindowHandler>(cancellationToken);
    }
}
