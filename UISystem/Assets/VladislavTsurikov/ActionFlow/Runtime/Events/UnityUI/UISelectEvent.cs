using UnityEngine.EventSystems;

namespace VladislavTsurikov.ActionFlow.Runtime.Events.UnityUI
{
    [EventCallbacksType(typeof(SelectCallbacks))]
    public abstract class UISelectEvent : Event
    {
        protected internal virtual void OnSelect(BaseEventData eventData)
        {
        }

        protected internal virtual void OnDeselect(BaseEventData eventData)
        {
        }
    }
}
