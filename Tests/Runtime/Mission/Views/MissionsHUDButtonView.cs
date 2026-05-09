#if UI_SYSTEM_UNIRX
using System;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using VladislavTsurikov.UISystem.Runtime.Core;

namespace VladislavTsurikov.UISystem.Tests.Runtime
{
    public class MissionsHUDButtonView : MonoBehaviour, IBindableView
    {
        [SerializeField]
        private Button _button;

        [SerializeField]
        private TextMeshProUGUI _labelText;

        public IObservable<Unit> OnClicked => _button.OnClickAsObservable();

        public Button Button => _button;

        public string BindingId => "MissionsHUDButtonView";

        public void SetLabelText(string text)
        {
            if (_labelText != null)
            {
                _labelText.text = text;
            }
        }
    }
}

#endif
