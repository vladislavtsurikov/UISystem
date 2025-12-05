using System;
using UnityEngine;
using VladislavTsurikov.UnityUtility.Runtime;

namespace VladislavTsurikov.PhysicsSimulator.Runtime
{
    public static class PhysicsSimulator
    {
        public static bool UseAccelerationPhysics = true;

        static PhysicsSimulator() => EditorAndRuntimeUpdate.AddUpdateFunction(SimulatePhysicsIfNecessary);

        public static DisablePhysicsMode ActiveDisablePhysicsMode { get; private set; } =
            new ObjectTimeDisablePhysicsMode();

        private static void SimulatePhysicsIfNecessary()
        {
            if (SimulatedBodyStack.SimulatedBodyHashSet.Count == 0)
            {
                return;
            }

            ActiveDisablePhysicsMode.DisablePhysicsSupport();

            if (PhysicsSimulatorSettings.Instance.SimulatePhysics)
            {
                SimulatePhysics();
            }
        }

        private static void SimulatePhysics()
        {
#if UNITY_2022_3_OR_NEWER
            SimulationMode simulationMode = Physics.simulationMode;

            Physics.simulationMode = SimulationMode.Script;
#else
            bool prevAutoSimulation = Physics.autoSimulation;

            Physics.autoSimulation = false;
#endif

            float accelerationPhysics = UseAccelerationPhysics ? PhysicsSimulatorSettings.Instance.SpeedUpPhysics : 1;

            for (var i = 0; i < accelerationPhysics; i++)
            {
                Physics.Simulate(Time.fixedDeltaTime);

                SimulatedBodyStack.DisablePhysicsSupportIfObjectStopped();

                if (SimulatedBodyStack.SimulatedBodyHashSet.Count == 0)
                {
                    break;
                }
            }

#if UNITY_2022_3_OR_NEWER
            Physics.simulationMode = simulationMode;
#else
            Physics.autoSimulation = prevAutoSimulation;
#endif
        }

        public static void SetDisablePhysicsMode<T>() where T : DisablePhysicsMode
        {
            if (ActiveDisablePhysicsMode.GetType() != typeof(T))
            {
                ActiveDisablePhysicsMode.OnDisable();

                ActiveDisablePhysicsMode = Activator.CreateInstance<T>();
            }
        }

        public static void ResetDisablePhysicsMode() => ActiveDisablePhysicsMode?.Reset();
    }
}
