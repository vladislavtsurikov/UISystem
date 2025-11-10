using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace VladislavTsurikov.ActionFlow.Runtime.Events.UnityUI
{
    [RequireComponent(typeof(Graphic))]
    public class PointerCallbacks : EventCallbacks, IPointerEnterHandler, IPointerExitHandler
    {
        private UIPointerEvent UIPointerEvent => (UIPointerEvent)TriggerEvent;

        public void OnPointerEnter(PointerEventData eventData) => UIPointerEvent?.OnHoverEnter(eventData);

        public void OnPointerExit(PointerEventData eventData) => UIPointerEvent?.OnHoverExit(eventData);
    }
}
