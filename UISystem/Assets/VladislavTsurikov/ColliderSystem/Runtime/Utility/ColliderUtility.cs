using System.Collections.Generic;
using UnityEngine;
using VladislavTsurikov.SceneDataSystem.Runtime;
using VladislavTsurikov.SceneDataSystem.Runtime.Utility;

namespace VladislavTsurikov.ColliderSystem.Runtime
{
    public static class ColliderUtility
    {
        public static RayHit Raycast(Ray ray, LayerMask layerMask)
        {
            var raycastFilter = new ObjectFilter { LayerMask = layerMask };

            return Raycast(ray, raycastFilter);
        }

        public static RayHit Raycast<T>(Ray ray, LayerMask layerMask)
            where T : RendererSceneData, IRaycast
        {
            var raycastFilter = new ObjectFilter { LayerMask = layerMask };

            return Raycast<T>(ray, raycastFilter);
        }

        public static RayHit Raycast(Ray ray, ObjectFilter raycastFilter)
        {
            List<RayHit> allObjectHits = RaycastAll(ray, raycastFilter);

            RayHit.SortByHitDistance(allObjectHits);
            RayHit closestObjectHit = allObjectHits.Count != 0 ? allObjectHits[0] : null;

            return closestObjectHit;
        }

        public static RayHit Raycast<T>(Ray ray, ObjectFilter raycastFilter)
            where T : RendererSceneData, IRaycast
        {
            List<RayHit> allObjectHits = RaycastAll<T>(ray, raycastFilter);

            RayHit.SortByHitDistance(allObjectHits);
            RayHit closestObjectHit = allObjectHits.Count != 0 ? allObjectHits[0] : null;

            return closestObjectHit;
        }

        public static List<RayHit> RaycastAll<T>(Ray ray, ObjectFilter raycastFilter)
            where T : RendererSceneData, IRaycast
        {
            List<SceneDataManager> sceneDataManagers = SceneDataManagerFinder.RaycastAll(ray);

            var allObjectHits = new List<RayHit>();

            foreach (SceneDataManager sceneDataManager in sceneDataManagers)
            {
                T rendererSceneData = SceneDataStackUtility.InstanceSceneData<T>(sceneDataManager.Scene);

                if (rendererSceneData != null)
                {
                    allObjectHits.AddRange(rendererSceneData.RaycastAll(ray, raycastFilter));
                }
            }

            return allObjectHits;
        }

        public static List<RayHit> RaycastAll(Ray ray, ObjectFilter raycastFilter)
        {
            List<SceneDataManager> sceneDataManagers = SceneDataManagerFinder.RaycastAll(ray);

            var allObjectHits = new List<RayHit>();

            foreach (SceneDataManager sceneDataManager in sceneDataManagers)
            foreach (SceneData sceneData in sceneDataManager.SceneDataStack.ElementList)
            {
                if (sceneData is IRaycast rendererSceneData)
                {
                    List<RayHit> rayHits = rendererSceneData.RaycastAll(ray, raycastFilter);

                    if (rayHits != null)
                    {
                        allObjectHits.AddRange(rendererSceneData.RaycastAll(ray, raycastFilter));
                    }
                }
            }

            return allObjectHits;
        }
    }
}
