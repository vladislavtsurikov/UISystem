using System;
using OdinSerializer;
using UnityEngine.UI;
using VladislavTsurikov.EntityDataAction.Runtime;
using VladislavTsurikov.EntityDataAction.Runtime.Core;
using VladislavTsurikov.ReflectionUtility;

namespace VladislavTsurikov.EntityDataAction.Shared.Runtime.Common
{
    [Name("UI/Common/ButtonData")]
    public sealed class ButtonData : ComponentData
    {
        [OdinSerialize]
        public Button Button;

        public override string Name => Button != null ? $"Button ({Button.name})" : "Button";

        public event Action<Entity> OnClick;

        protected override void SetupComponent(object[] setupData = null)
        {
            if (Button != null)
            {
                Button.onClick.AddListener(HandleClick);
            }
        }

        protected override void OnDisableElement()
        {
            if (Button != null)
            {
                Button.onClick.RemoveListener(HandleClick);
            }
        }

        private void HandleClick()
        {
            int subscriberCount = OnClick?.GetInvocationList()?.Length ?? 0;
            OnClick?.Invoke(Entity);
        }
    }
}
