using UnityEngine;

namespace VladislavTsurikov.ActionFlow.Runtime.Events.Physics
{
    [RequireComponent(typeof(Collider))]
    public class PhysicsCallbacks : EventCallbacks
    {
        private PhysicsEvent _physicsEvent => (PhysicsEvent)TriggerEvent;

        private void OnCollisionEnter(Collision collision) => _physicsEvent?.OnCollisionEnter(collision);

        private void OnCollisionExit(Collision collision) => _physicsEvent?.OnCollisionExit(collision);

        private void OnCollisionStay(Collision collision) => _physicsEvent?.OnCollisionStay(collision);

        private void OnTriggerEnter(Collider other) => _physicsEvent?.OnTriggerEnter(other);

        private void OnTriggerExit(Collider other) => _physicsEvent?.OnTriggerExit(other);

        private void OnTriggerStay(Collider other) => _physicsEvent?.OnTriggerStay(other);
    }
}
