#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.ComponentStack.Runtime.Core;
using VladislavTsurikov.RendererStack.Runtime.Core;
using VladislavTsurikov.RendererStack.Runtime.Core.SceneSettings.Camera;
using VladislavTsurikov.SceneDataSystem.Runtime.StreamingUtility;
using VladislavTsurikov.UnityUtility.Editor;

namespace VladislavTsurikov.RendererStack.Runtime.Sectorize
{
    public partial class Sectorize
    {
        public override void DrawDebug()
        {
            if (DebugAllCells)
            {
                Handles.color = Color.blue;

                List<Sector> sectors = StreamingUtility.GetAllScenes(GetSectorLayerTag());

                if (sectors != null)
                {
                    foreach (Sector sector in sectors)
                    {
                        Handles.DrawWireCube(sector.Bounds.center, sector.Bounds.size);
                    }
                }
            }

            if (DebugVisibleCells)
            {
                foreach (VirtualCamera cam in CameraManager.VirtualCameraList)
                {
                    if (cam.Ignored)
                    {
                        continue;
                    }

                    if (RendererStackManager.Instance.EditorPlayModeSimulation || Application.isPlaying)
                    {
                        if (PreventingUnloading.IsValid())
                        {
                            if (PreventingUnloading.MaxDistance != 0)
                            {
                                Handles.color = Color.yellow;
                                DrawHandles.CircleCap(1, cam.Camera.transform.position,
                                    Quaternion.LookRotation(Vector3.up), PreventingUnloading.MaxDistance);
                            }
                        }

                        if (AsynchronousLoading.IsValid())
                        {
                            if (AsynchronousLoading.MaxDistance != 0)
                            {
                                Handles.color = Color.blue;
                                DrawHandles.CircleCap(1, cam.Camera.transform.position,
                                    Quaternion.LookRotation(Vector3.up), AsynchronousLoading.MaxDistance);
                            }
                        }

                        if (ImmediatelyLoading.MaxDistance != 0)
                        {
                            Handles.color = Color.red;
                            DrawHandles.CircleCap(1, cam.Camera.transform.position, Quaternion.LookRotation(Vector3.up),
                                ImmediatelyLoading.MaxDistance);
                        }
                    }

                    foreach (Sector sector in FindSector.OverlapSphere(cam.Camera.transform.position,
                                 GetMaxLoadingDistance(), GetSectorLayerTag(), false))
                    {
                        Handles.color = Color.green;
                        Handles.DrawWireCube(sector.Bounds.center, sector.Bounds.size);
                    }
                }
            }
        }
    }
}
#endif
