using UnityEngine;
using UnityEngine.UI;
using VladislavTsurikov.UISystem.Runtime.UnityUIIntegration;

namespace VladislavTsurikov.UISystem.Tests.Runtime
{
    public class GeneralMissionView : MonoBehaviour, IBindableUIComponent
    {
        [SerializeField]
        private ScrollRect _rewardsScrollView;

        [field: SerializeField]
        public RectTransform MissionSpawnRect { get; private set; }

        public string BindingId => "GeneralMissionView";
    }
}
