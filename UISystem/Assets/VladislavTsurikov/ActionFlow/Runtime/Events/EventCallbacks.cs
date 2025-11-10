using OdinSerializer;
using UnityEngine;

namespace VladislavTsurikov.ActionFlow.Runtime.Events
{
    public abstract class EventCallbacks : MonoBehaviour
    {
        [OdinSerialize]
        protected Event TriggerEvent;

        public void Setup(Event triggerEvent) => TriggerEvent = triggerEvent;
    }
}
