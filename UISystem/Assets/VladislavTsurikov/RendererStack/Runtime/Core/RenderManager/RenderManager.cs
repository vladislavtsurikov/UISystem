using System;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.Rendering;
using VladislavTsurikov.ComponentStack.Runtime.Core;
using VladislavTsurikov.RendererStack.Runtime.Common.GlobalSettings;
using VladislavTsurikov.RendererStack.Runtime.Common.PrototypeSettings;
using VladislavTsurikov.RendererStack.Runtime.Core.PrototypeRendererSystem;
using VladislavTsurikov.RendererStack.Runtime.Core.RenderManager.GPUInstancedIndirect;
using VladislavTsurikov.RendererStack.Runtime.Core.SceneSettings.Camera;
#if BillboardSystem
using VladislavTsurikov.InstantRenderer.LargeObjectRenderer.Scripts.RendererData.BillboardSystem;
#endif

namespace VladislavTsurikov.RendererStack.Runtime.Core.RenderManager
{
    public static class RenderManager
    {
        private static readonly GPUInstancedIndirectRenderer _gpuInstancedIndirectRenderer = new();
#if BillboardSystem
        public static RenderImposterCells RenderImposterCells = new RenderImposterCells();
#endif

        public static void Render(Type rendererType, SelectionData selectionData)
        {
            Profiler.BeginSample("Render");

            if (!_gpuInstancedIndirectRenderer.IsSetup)
            {
                _gpuInstancedIndirectRenderer.Setup();
            }

            var cameraManager =
                (CameraManager)RendererStackManager.Instance.SceneComponentStack.GetElement(typeof(CameraManager));

            for (var cameraIndex = 0; cameraIndex <= cameraManager.VirtualCameraList.Count - 1; cameraIndex++)
            {
                if (cameraManager.VirtualCameraList[cameraIndex].Ignored)
                {
                    continue;
                }

                Camera targetCamera = cameraManager.VirtualCameraList[cameraIndex].GetRenderingCamera();

                for (var protoIndex = 0; protoIndex <= selectionData.PrototypeList.Count - 1; protoIndex++)
                {
                    Prototype proto = selectionData.PrototypeList[protoIndex];

                    if (proto.Active == false)
                    {
                        continue;
                    }

                    if (Utility.Utility.IsInLayer(cameraManager.VirtualCameraList[cameraIndex].Camera.cullingMask,
                            proto.Layer) == false)
                    {
                        continue;
                    }

                    var shadowSettings = (Shadow)proto.GetSettings(typeof(Shadow));

                    var quality =
                        (Quality)GlobalSettings.GlobalSettings.Instance.GetElement(typeof(Quality), rendererType);

                    ShadowCastingMode shadowCastingMode = quality.GetShadowCastingMode();

                    if (!shadowSettings.IsValid())
                    {
                        shadowCastingMode = ShadowCastingMode.Off;
                    }

                    var distanceCulling = (DistanceCulling)proto.GetSettings(typeof(DistanceCulling));

                    var maxDistance = Utility.Utility.GetMaxDistance(rendererType,
                        cameraManager.VirtualCameraList[cameraIndex], distanceCulling);

#if BillboardSystem
                    RenderImposterCells.Render(selectionData, protoIndex, cameraIndex, targetCamera, shadowCastingMode, maxDistance);
#endif

                    _gpuInstancedIndirectRenderer.Render(selectionData, protoIndex, cameraIndex, targetCamera,
                        shadowCastingMode, maxDistance);
                }
            }

            Profiler.EndSample();
        }
    }
}
