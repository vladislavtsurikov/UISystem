using UnityEngine;
using UnityEngine.UI;
using VladislavTsurikov.UISystem.Runtime.UnityUIIntegration;

namespace VladislavTsurikov.UISystem.Tests.Runtime
{
    public class MissionWindowView : MonoBehaviour, IBindableUIComponent
    {
        [SerializeField]
        private ScrollRect _rewardsScrollView;

        [field: SerializeField]
        public RectTransform MissionSpawnRect { get; private set; }

        [field: SerializeField]
        public VerticalLayoutGroup VerticalGroup { get; private set; }

        public string BindingId => "MissionWindowView";
    }
}
