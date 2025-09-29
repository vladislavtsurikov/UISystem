using System;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using VladislavTsurikov.UISystem.Runtime.UnityUIIntegration;

namespace VladislavTsurikov.UISystem.Tests.Runtime
{
    public class UIMissionToggleView : MonoBehaviour, IBindableUIComponent
    {
        [Header("UI")]
        [SerializeField]
        private GameObject _redCircleObject;

        [SerializeField]
        private TextMeshProUGUI _redCircleAmountText;

        [SerializeField]
        private TextMeshProUGUI _toggleText;

        [SerializeField]
        protected Toggle Toggle;

        [Header("Binding ID")]
        [SerializeField]
        private string _bindingId;

        public bool ActiveState => gameObject.activeSelf;
        public IObservable<bool> OnClicked => Toggle.OnValueChangedAsObservable();

        public string BindingId => _bindingId;

        public void SetActive(bool value) => gameObject.SetActive(value);
        public void SetActiveRedCircle(bool value) => _redCircleObject.SetActive(value);
        public void SetRedCircleAmount(string amount) => _redCircleAmountText.text = amount;
    }
}
