using VladislavTsurikov.ReflectionUtility;

namespace VladislavTsurikov.ActionFlow.Runtime.Events.GameObjectLifecycle
{
    [Name("Lifecycle/On Disable")]
    public class OnDisableEvent : LifecycleEvent
    {
        protected internal override void OnDisable() => Trigger.Run();
    }
}
