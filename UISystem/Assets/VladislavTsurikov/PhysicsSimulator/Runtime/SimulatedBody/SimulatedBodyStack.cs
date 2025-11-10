using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace VladislavTsurikov.PhysicsSimulator.Runtime
{
    public static class SimulatedBodyStack
    {
        public delegate void OnDisableAllSimulatedBodyDelegate();

        internal static readonly HashSet<SimulatedBody> SimulatedBodyHashSet = new(2000);
        public static OnDisableAllSimulatedBodyDelegate OnDisableAllSimulatedBody;

        public static int Count => SimulatedBodyHashSet.Count;

        public static T InstantiateSimulatedBody<T>(GameObject prefab, Vector3 position, Vector3 scaleFactor,
            Quaternion rotation, List<OnDisableSimulatedBodyEvent> onDisablePhysicsActions = null)
            where T : SimulatedBody
        {
#if UNITY_EDITOR
            var gameObject = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
#else
            var gameObject = Object.Instantiate(prefab);
#endif

            gameObject.transform.position = position;
            gameObject.transform.localScale = scaleFactor;
            gameObject.transform.rotation = rotation;

            var simulatedBody = (T)Activator.CreateInstance(typeof(T), gameObject, onDisablePhysicsActions);

            RegisterSimulatedBody(simulatedBody, false);

            return simulatedBody;
        }

        public static SimulatedBody InstantiateSimulatedBody(GameObject prefab, Vector3 position, Vector3 scaleFactor,
            Quaternion rotation, List<OnDisableSimulatedBodyEvent> onDisablePhysicsActions = null)
        {
#if UNITY_EDITOR
            var gameObject = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
#else
            var gameObject = Object.Instantiate(prefab);
#endif

            gameObject.transform.position = position;
            gameObject.transform.localScale = scaleFactor;
            gameObject.transform.rotation = rotation;

            var simulatedBody = new SimulatedBody(gameObject, onDisablePhysicsActions);

            RegisterSimulatedBody(simulatedBody, false);

            return simulatedBody;
        }

        public static void RegisterSimulatedBody(SimulatedBody simulatedBody, bool checkForSameGameObject = true)
        {
            if (checkForSameGameObject)
            {
                if (GetSimulatedBody(simulatedBody.GameObject) != null)
                {
                    return;
                }
            }

            simulatedBody.OnAddToSimulatedBodyStack?.Invoke();
            SimulatedBodyHashSet.Add(simulatedBody);

            PhysicsSimulator.ActiveDisablePhysicsMode.OnRegisterSimulatedBody(simulatedBody);
        }

        public static void DisablePhysicsSupportIfObjectStopped()
        {
            if (!PhysicsSimulatorSettings.Instance.SimulatePhysics)
            {
                return;
            }

            var removeSimulatedBodyList = new List<SimulatedBody>();

            foreach (SimulatedBody simulatedBody in SimulatedBodyHashSet)
            {
                if (simulatedBody.IsRigidbodyStopping())
                {
                    removeSimulatedBodyList.Add(simulatedBody);
                }
            }

            foreach (SimulatedBody simulatedBody in removeSimulatedBodyList)
            {
                DisablePhysicsSupport(simulatedBody);
            }
        }

        public static SimulatedBody GetSimulatedBody(GameObject gameObject)
        {
            foreach (SimulatedBody simulatedBody in SimulatedBodyHashSet)
            {
                if (simulatedBody.GameObject == null)
                {
                    continue;
                }

                if (simulatedBody.GameObject == gameObject)
                {
                    return simulatedBody;
                }
            }

            return null;
        }

        public static void DisableAllPhysicsSupport()
        {
            var removeSimulatedBodyList = new List<SimulatedBody>();
            removeSimulatedBodyList.AddRange(SimulatedBodyHashSet);

            foreach (SimulatedBody simulatedBody in removeSimulatedBodyList)
            {
                DisablePhysicsSupport(simulatedBody);
            }
        }

        public static void DisablePhysicsSupport(SimulatedBody simulatedBody)
        {
            if (simulatedBody.GameObject == null || !simulatedBody.HasRigidbody())
            {
                SimulatedBodyHashSet.Remove(simulatedBody);
                return;
            }

            SimulatedBodyHashSet.Remove(simulatedBody);
            PhysicsSimulator.ActiveDisablePhysicsMode.OnUnregisterSimulatedBody(simulatedBody);

            simulatedBody.DisablePhysicsSupport();

            if (SimulatedBodyHashSet.Count == 0)
            {
                OnDisableAllSimulatedBody?.Invoke();
            }
        }
    }
}
