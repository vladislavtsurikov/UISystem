#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEngine;
using VladislavTsurikov.ColliderSystem.Runtime;
using VladislavTsurikov.GameObjectCollider.Editor;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.Utility;

namespace VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.PrototypeGameObject
{
    public static class PrototypeGameObjectOverlap
    {
        public static void OverlapSphere(Vector3 sphereCenter, float sphereRadius,
            Func<PrototypeGameObject, GameObject, bool> func)
        {
            var overlapObjectFilter = new ObjectFilter { FindOnlyInstancePrefabs = true };

            List<ColliderObject> overlappedObjects =
                GameObjectColliderUtility.OverlapSphere(sphereCenter, sphereRadius, overlapObjectFilter);

            foreach (ColliderObject colliderObject in overlappedObjects)
            {
                var go = (GameObject)colliderObject.Obj;

                if (go == null)
                {
                    continue;
                }

                List<PrototypeGameObject> prototypes =
                    GetPrototypeUtility.GetPrototypes<PrototypeGameObject>(colliderObject.GetPrefab());

                if (prototypes.Count == 0)
                {
                    continue;
                }

                foreach (PrototypeGameObject prototype in prototypes)
                {
                    if (!func.Invoke(prototype, go))
                    {
                        return;
                    }
                }
            }
        }

        public static void OverlapBox(Bounds bounds, Func<PrototypeGameObject, GameObject, bool> func) =>
            OverlapBox(bounds.center, bounds.size, Quaternion.identity, func);

        public static void OverlapBox(Vector3 boxCenter, Vector3 boxSize, Quaternion boxRotation,
            Func<PrototypeGameObject, GameObject, bool> func)
        {
            var overlapObjectFilter = new ObjectFilter { FindOnlyInstancePrefabs = true };

            List<ColliderObject> overlappedObjects =
                GameObjectColliderUtility.OverlapBox(boxCenter, boxSize, boxRotation, overlapObjectFilter);

            foreach (ColliderObject colliderObject in overlappedObjects)
            {
                var go = (GameObject)colliderObject.Obj;

                if (go == null)
                {
                    continue;
                }

                List<PrototypeGameObject> prototypes =
                    GetPrototypeUtility.GetPrototypes<PrototypeGameObject>(colliderObject.GetPrefab());

                if (prototypes.Count == 0)
                {
                    continue;
                }

                foreach (PrototypeGameObject prototype in prototypes)
                {
                    if (!func.Invoke(prototype, go))
                    {
                        return;
                    }
                }
            }
        }
    }
}
#endif
