using UnityEngine;

namespace VladislavTsurikov.ActionFlow.Runtime.Events.Physics
{
    [EventCallbacksType(typeof(PhysicsCallbacks))]
    public abstract class PhysicsEvent : Event
    {
        protected internal virtual void OnTriggerEnter(Collider other)
        {
        }

        protected internal virtual void OnTriggerExit(Collider other)
        {
        }

        protected internal virtual void OnTriggerStay(Collider other)
        {
        }

        protected internal virtual void OnCollisionEnter(Collision collision)
        {
        }

        protected internal virtual void OnCollisionExit(Collision collision)
        {
        }

        protected internal virtual void OnCollisionStay(Collision collision)
        {
        }
    }
}
