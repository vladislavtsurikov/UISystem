namespace VladislavTsurikov.ActionFlow.Runtime.Events.GameObjectLifecycle
{
    [EventCallbacksType(typeof(LifecycleEventCallbacks))]
    public abstract class LifecycleEvent : Event
    {
        protected internal virtual void Awake()
        {
        }

        protected internal virtual void Start()
        {
        }

        protected internal virtual void OnEnable()
        {
        }

        protected internal virtual void OnDisable()
        {
        }

        protected internal virtual void Update()
        {
        }

        protected internal virtual void FixedUpdate()
        {
        }

        protected internal virtual void LateUpdate()
        {
        }
    }
}
