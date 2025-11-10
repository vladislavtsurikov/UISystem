#if RENDERER_STACK
using System;
using System.Collections.Generic;
using UnityEngine;
using VladislavTsurikov.ColliderSystem.Runtime;
using VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer;
using VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.Data;

namespace VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.PrototypeTerrainObject
{
    public static class PrototypeTerrainObjectOverlap
    {
        public static void OverlapSphere(Vector3 sphereCenter, float sphereRadius, ObjectFilter objectFilter,
            bool quadTree, bool checkObbIntersection, Func<PrototypeTerrainObject, TerrainObjectInstance, bool> func) =>
            TerrainObjectRendererAPI.OverlapSphere(sphereCenter, sphereRadius, objectFilter, quadTree,
                checkObbIntersection, largeObjectInstance =>
                {
                    List<PrototypeTerrainObject> prototypes =
                        AllAvailableTerrainObjectPrototypes.GetPrototypes(largeObjectInstance.PrototypeID);

                    if (prototypes.Count == 0)
                    {
                        return true;
                    }

                    foreach (PrototypeTerrainObject prototype in prototypes)
                    {
                        func.Invoke(prototype, largeObjectInstance);
                    }

                    return true;
                });

        public static void OverlapBox(Bounds bounds, ObjectFilter objectFilter, bool quadTree,
            bool checkObbIntersection,
            Func<PrototypeTerrainObject, TerrainObjectInstance, bool> func) =>
            OverlapBox(bounds.center, bounds.size, Quaternion.identity, objectFilter, quadTree, checkObbIntersection,
                func);

        public static void OverlapBox(Vector3 boxCenter, Vector3 boxSize, Quaternion boxRotation,
            ObjectFilter objectFilter,
            bool quadTree, bool checkObbIntersection, Func<PrototypeTerrainObject, TerrainObjectInstance, bool> func) =>
            TerrainObjectRendererAPI.OverlapBox(boxCenter, boxSize, boxRotation, objectFilter, quadTree,
                checkObbIntersection, largeObjectInstance =>
                {
                    List<PrototypeTerrainObject> prototypes =
                        AllAvailableTerrainObjectPrototypes.GetPrototypes(largeObjectInstance.PrototypeID);

                    if (prototypes == null || prototypes.Count == 0)
                    {
                        return true;
                    }

                    foreach (PrototypeTerrainObject prototype in prototypes)
                    {
                        if (!func.Invoke(prototype, largeObjectInstance))
                        {
                            return false;
                        }
                    }

                    return true;
                });
    }
}
#endif
