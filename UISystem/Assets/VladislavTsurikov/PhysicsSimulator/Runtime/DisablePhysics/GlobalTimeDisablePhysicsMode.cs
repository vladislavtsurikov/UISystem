using UnityEngine;

namespace VladislavTsurikov.PhysicsSimulator.Runtime
{
    public class GlobalTimeDisablePhysicsMode : DisablePhysicsMode
    {
        private float _timeToFinish;

        internal override void DisablePhysicsSupport()
        {
            if (_timeToFinish <= Time.time)
            {
                _timeToFinish = Time.time + PhysicsSimulatorSettings.Instance.GlobalDisablePhysicsTime;
                SimulatedBodyStack.DisableAllPhysicsSupport();
            }
        }

        internal override void Reset() =>
            _timeToFinish = Time.time + PhysicsSimulatorSettings.Instance.GlobalDisablePhysicsTime;
    }
}
