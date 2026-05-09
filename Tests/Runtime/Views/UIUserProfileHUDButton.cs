#if UI_SYSTEM_UNIRX
using System;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using VladislavTsurikov.UISystem.Runtime.Core;

namespace VladislavTsurikov.UISystem.Tests.Runtime
{
    public class UIUserProfileHUDButton : MonoBehaviour, IBindableView
    {
        [SerializeField]
        private Button _button;

        [SerializeField]
        private Image _userAvatar;

        public IObservable<Unit> OnClick => _button.OnClickAsObservable();

        public string BindingId => "UIUserProfileHUD";

        public void SetUserAvatar(Sprite sprite)
        {
            _userAvatar.sprite = sprite;
        }
    }
}

#endif
