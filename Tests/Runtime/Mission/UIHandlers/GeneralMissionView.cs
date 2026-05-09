using UnityEngine;
using UnityEngine.UI;
using VladislavTsurikov.UISystem.Runtime.Core;

namespace VladislavTsurikov.UISystem.Tests.Runtime
{
    public class GeneralMissionView : MonoBehaviour, IBindableView
    {
        [SerializeField]
        private ScrollRect _rewardsScrollView;

        [field: SerializeField]
        public RectTransform MissionSpawnRect { get; private set; }

        public string BindingId => "GeneralMissionView";
    }
}
