using UnityEngine;
using VladislavTsurikov.ReflectionUtility;

namespace VladislavTsurikov.ActionFlow.Runtime.Events.Physics
{
    [Name("Physics/On Collision Exit")]
    public class OnCollisionExitEvent : PhysicsEvent
    {
        protected internal override void OnCollisionExit(Collision collision) => Trigger.Run();
    }
}
