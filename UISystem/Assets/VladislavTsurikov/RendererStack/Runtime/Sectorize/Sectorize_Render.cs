using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using VladislavTsurikov.ComponentStack.Runtime.Core;
using VladislavTsurikov.Math.Runtime;
using VladislavTsurikov.RendererStack.Runtime.Core.SceneSettings.Camera;
using VladislavTsurikov.SceneDataSystem.Runtime.StreamingUtility;

namespace VladislavTsurikov.RendererStack.Runtime.Sectorize
{
    public partial class Sectorize
    {
        private Scene _lastLoadedScene;

        public override void Render()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                if (EnableManualSceneControl)
                {
                    return;
                }
            }
#endif

            foreach (VirtualCamera cam in CameraManager.VirtualCameraList)
            {
                if (cam.Ignored)
                {
                    continue;
                }

                List<Sector> loadSectors = FindSector.OverlapSphere(cam.Camera.transform.position,
                    GetMaxLoadingDistance(),
                    GetSectorLayerTag(), false);

                if (loadSectors == null)
                {
                    continue;
                }

                foreach (Sector sector in loadSectors)
                {
                    sector.SceneReference.UnloadSceneCancellationTokenSource?.Cancel();

                    if (sector.IsLoaded)
                    {
                        continue;
                    }

                    var loadImmediatelySphere =
                        new Sphere(cam.Camera.transform.position, ImmediatelyLoading.MaxDistance);

                    if (loadImmediatelySphere.Intersects(new AABB(sector.Bounds.center, sector.Bounds.size)))
                    {
                        sector.LoadScene();
                        continue;
                    }

                    if (!_lastLoadedScene.IsValid() || _lastLoadedScene.isLoaded)
                    {
                        sector.LoadScene(
                            CalculateMaximumPauseBeforeLoadingScene(sector, cam.Camera.transform.position));
                        _lastLoadedScene = sector.SceneReference.Scene;
                    }
                }

                var preventingUnloadSceneSphere = new Sphere(cam.Camera.transform.position,
                    PreventingUnloading.IsValid() ? PreventingUnloading.MaxDistance : 0);

                UnloadUnnecessaryScenes(loadSectors, preventingUnloadSceneSphere);
            }
        }

        private void UnloadUnnecessaryScenes(List<Sector> loadSectors, Sphere preventingUnloadSceneSphere)
        {
            List<Sector> allLoadedScenes = SectorLayerManager.Instance.GetLoadedScenes(GetSectorLayerTag());

            if (allLoadedScenes == null)
            {
                return;
            }

            var unloadSectors = new List<Sector>(allLoadedScenes);
            unloadSectors.RemoveAll(loadSectors.Contains);

            foreach (Sector sector in unloadSectors)
            {
                if (!preventingUnloadSceneSphere.Intersects(new AABB(sector.Bounds.center, sector.Bounds.size)))
                {
                    if (Caching.IsValid())
                    {
                        sector.CacheScene(0, Caching.KeepScenes);
                    }
                    else
                    {
                        sector.UnloadScene();
                    }
                }
            }
        }

        public float GetMaxLoadingDistance() =>
            Mathf.Max(ImmediatelyLoading.MaxDistance,
                AsynchronousLoading.IsValid() ? AsynchronousLoading.MaxDistance : 0);

        private float CalculateMaximumPauseBeforeLoadingScene(Sector sector, Vector3 center)
        {
            var distance = Vector3.Distance(sector.Bounds.center, center);
            var t = Mathf.InverseLerp(GetMaxLoadingDistance(), ImmediatelyLoading.MaxDistance, distance);

            return Mathf.Lerp(GetMaxPauseBeforeLoadingScene(sector), 0, t);
        }

        private float GetMaxPauseBeforeLoadingScene(Sector sector)
        {
            if (Caching.IsValid() && sector.CachedScene)
            {
                return Caching.MaxLoadingCachedScenePause;
            }

            if (AsynchronousLoading.IsValid())
            {
                return AsynchronousLoading.MaxLoadingScenePause;
            }

            return 0;
        }
    }
}
