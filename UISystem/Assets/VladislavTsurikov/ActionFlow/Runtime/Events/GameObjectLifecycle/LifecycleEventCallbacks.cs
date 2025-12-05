namespace VladislavTsurikov.ActionFlow.Runtime.Events.GameObjectLifecycle
{
    public class LifecycleEventCallbacks : EventCallbacks
    {
        private LifecycleEvent LifecycleEvent => (LifecycleEvent)TriggerEvent;

        private void Awake() => LifecycleEvent.Awake();

        private void Start() => LifecycleEvent.Start();

        private void Update() => LifecycleEvent.Update();

        private void FixedUpdate() => LifecycleEvent.FixedUpdate();

        private void LateUpdate() => LifecycleEvent.LateUpdate();

        private void OnEnable() => LifecycleEvent.OnEnable();

        private void OnDisable() => LifecycleEvent.OnDisable();
    }
}
