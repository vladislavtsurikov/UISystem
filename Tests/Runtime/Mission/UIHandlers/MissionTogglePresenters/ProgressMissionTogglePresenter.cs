#if UI_SYSTEM_ADDRESSABLE_LOADER_SYSTEM
#if UI_SYSTEM_ZENJECT
using System.Threading;
using Cysharp.Threading.Tasks;
using VladislavTsurikov.AddressableLoaderSystem.Runtime.Core;
using VladislavTsurikov.UISystem.Runtime.Core;

namespace VladislavTsurikov.UISystem.Tests.Runtime.MissionTogglePresenters
{
    [SceneFilter("TestScene_1")]
    [UIParent(typeof(UIMissionsMainWindowPresenter))]
    public class ProgressMissionTogglePresenter : UIMissionTogglePresenter
    {
        protected override bool UnlockedTab => true;

        protected override int NotificationCount => 10;

        protected override string ToggleBindingId => "Progress";

        protected override async UniTask OnToggleClicked(CancellationToken cancellationToken)
        {
            await UINavigator.Show<ProgressMissionsWindowPresenter>(cancellationToken);
        }
    }
}

#endif

#endif
