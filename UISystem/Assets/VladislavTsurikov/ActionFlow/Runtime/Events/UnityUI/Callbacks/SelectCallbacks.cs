using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace VladislavTsurikov.ActionFlow.Runtime.Events.UnityUI
{
    [RequireComponent(typeof(Selectable))]
    public class SelectCallbacks : EventCallbacks, ISelectHandler, IDeselectHandler
    {
        private UISelectEvent UISelectEvent => (UISelectEvent)TriggerEvent;

        public void OnDeselect(BaseEventData eventData) => UISelectEvent?.OnDeselect(eventData);

        public void OnSelect(BaseEventData eventData) => UISelectEvent?.OnSelect(eventData);
    }
}
