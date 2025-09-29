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
    public class ChapterMissionTogglePresenter : UIMissionTogglePresenter
    {
        public ChapterMissionTogglePresenter(DiContainer container)
            : base(container)
        {
        }


        protected override bool UnlockedTab => true;
        protected override int NotificationCount => 3;

        protected override UIMissionToggleView ResolveView() =>
            _container.ResolveId<UIMissionToggleView>(UIBindingId.FromTypeAndIndex(typeof(UIMissionsMainWindowHandler),
                "Chapter"));

        protected override async UniTask OnToggleClicked(CancellationToken cancellationToken) =>
            await UINavigator.Show<ChapterMissionsWindowHandler, UIMissionsMainWindowHandler>(cancellationToken);
    }
}
