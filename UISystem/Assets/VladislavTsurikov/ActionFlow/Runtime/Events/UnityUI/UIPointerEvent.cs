using UnityEngine.EventSystems;

namespace VladislavTsurikov.ActionFlow.Runtime.Events.UnityUI
{
    [EventCallbacksType(typeof(PointerCallbacks))]
    public abstract class UIPointerEvent : Event
    {
        protected internal virtual void OnHoverEnter(PointerEventData eventData)
        {
        }

        protected internal virtual void OnHoverExit(PointerEventData eventData)
        {
        }
    }
}
