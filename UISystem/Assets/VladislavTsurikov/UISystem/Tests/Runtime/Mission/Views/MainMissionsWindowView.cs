#if UI_SYSTEM_UNIRX
using System;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using VladislavTsurikov.UISystem.Runtime.UnityUIIntegration;

namespace VladislavTsurikov.UISystem.Tests.Runtime
{
    public class MainMissionsWindowView : MonoBehaviour, IBindableUIComponent
    {
        [SerializeField]
        private Button _closeButton;

        [field: SerializeField]
        public RectTransform MissionSpawnRect { get; private set; }


        public IObservable<Unit> OnCloseClicked => _closeButton.OnClickAsObservable();
        public ReactiveCommand<Unit> OnCloseCommand { get; } = new();

        public string BindingId => "MainMissionsWindowView";

        public void CloseView() => OnCloseCommand.Execute(Unit.Default);
    }
}

#endif
