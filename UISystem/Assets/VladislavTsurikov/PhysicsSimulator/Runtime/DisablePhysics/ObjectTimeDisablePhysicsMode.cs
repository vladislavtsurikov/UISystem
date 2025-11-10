using System.Collections.Generic;
using VladislavTsurikov.UnityTimer.Runtime;

namespace VladislavTsurikov.PhysicsSimulator.Runtime
{
    public class ObjectTimeDisablePhysicsMode : DisablePhysicsMode
    {
        private static bool _pastSimulatePhysics;

        private readonly Dictionary<SimulatedBody, Timer> _simulatedBodyTimers = new();

        internal override void DisablePhysicsSupport()
        {
            if (SimulatedBodyStack.SimulatedBodyHashSet.Count == 0)
            {
                return;
            }

            if (PhysicsSimulatorSettings.Instance.SimulatePhysics)
            {
                if (!_pastSimulatePhysics)
                {
                    foreach (SimulatedBody simulatedBody in SimulatedBodyStack.SimulatedBodyHashSet)
                    {
                        RegisterSimulatedBody(simulatedBody);
                    }
                }

                _pastSimulatePhysics = true;
            }
            else
            {
                if (_pastSimulatePhysics)
                {
                    foreach (SimulatedBody simulatedBody in SimulatedBodyStack.SimulatedBodyHashSet)
                    {
                        UnregisterSimulatedBody(simulatedBody);
                    }
                }

                _pastSimulatePhysics = false;
            }
        }

        internal override void OnDisable()
        {
            foreach (SimulatedBody simulatedBody in SimulatedBodyStack.SimulatedBodyHashSet)
            {
                UnregisterSimulatedBody(simulatedBody);
            }
        }

        internal override void OnRegisterSimulatedBody(SimulatedBody simulatedBody)
        {
            if (PhysicsSimulatorSettings.Instance.SimulatePhysics)
            {
                RegisterSimulatedBody(simulatedBody);
            }
        }

        internal override void OnUnregisterSimulatedBody(SimulatedBody simulatedBody) =>
            UnregisterSimulatedBody(simulatedBody);

        private void RegisterSimulatedBody(SimulatedBody simulatedBody) =>
            _simulatedBodyTimers.TryAdd(simulatedBody,
                Timer.Register(PhysicsSimulatorSettings.Instance.DisablePhysicsTime,
                    () => DisablePhysicsSupport(simulatedBody)));

        private void UnregisterSimulatedBody(SimulatedBody simulatedBody)
        {
            if (_simulatedBodyTimers.ContainsKey(simulatedBody))
            {
                Timer timer = _simulatedBodyTimers[simulatedBody];
                timer.Cancel();
                _simulatedBodyTimers.Remove(simulatedBody);
            }
        }

        private static void DisablePhysicsSupport(SimulatedBody simulatedBody) =>
            SimulatedBodyStack.DisablePhysicsSupport(simulatedBody);
    }
}
