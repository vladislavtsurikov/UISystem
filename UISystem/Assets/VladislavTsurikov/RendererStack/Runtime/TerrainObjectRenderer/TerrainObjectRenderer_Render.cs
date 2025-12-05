using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;
using VladislavTsurikov.RendererStack.Runtime.Core;
using VladislavTsurikov.RendererStack.Runtime.Core.Preferences;
using VladislavTsurikov.RendererStack.Runtime.Core.PrototypeRendererSystem;
using VladislavTsurikov.RendererStack.Runtime.Core.PrototypeRendererSystem.RenderModelData;
using VladislavTsurikov.RendererStack.Runtime.Core.RenderManager;
using VladislavTsurikov.RendererStack.Runtime.Core.SceneSettings.Camera;
using
    VladislavTsurikov.RendererStack.Runtime.Core.SceneSettings.Camera.CameraTemporarySettingsSystem.ObjectCameraRender;
using VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.Data;
using VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.Data.RendererData;
using VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.GPUInstancedIndirect;
using VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.SceneSettings;

namespace VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer
{
    public partial class TerrainObjectRenderer
    {
        public override void Render()
        {
            if (Active == false)
            {
                return;
            }

            UpdateRendererDataIfNecessary();

            RenderManager.Render(GetType(), SelectionData);
            ScriptingSystem.SetCollidersAroundCameras();
        }

        private void UpdateRendererDataIfNecessary()
        {
            var quality = (Quality)RendererStackManager.Instance.SceneComponentStack.GetElement(typeof(Quality));
            var cameraManager =
                (CameraManager)RendererStackManager.Instance.SceneComponentStack.GetElement(typeof(CameraManager));

            quality.UpdateFloatingOrigin();

            if (ModifiedPrototypeRenderDataStack.GetCount() != 0)
            {
                ModifiedPrototypeRenderDataStack.ConvertPersistentDataToTemporaryData();
                ForceUpdateRendererData = true;
            }

            if (RendererStackSettings.Instance.ForceUpdateRendererData)
            {
                ForceUpdateRendererData = true;
            }

            for (var i = 0; i < cameraManager.VirtualCameraList.Count; i++)
            {
                VirtualCamera cam = cameraManager.VirtualCameraList[i];

                if (cam.Ignored)
                {
                    continue;
                }

                if (cam.IsNeedUpdateRenderData(typeof(TerrainObjectRenderer)) || ForceUpdateRendererData)
                {
                    cam.RefreshCameraData(typeof(TerrainObjectRenderer));

                    List<Cell> visibleCellList = TerrainObjectRendererData.GetAllVisibleCellList(cam);
                    UpdateCameraRendererData(visibleCellList, i);
                }
            }

            ForceUpdateRendererData = false;
        }

        private void UpdateCameraRendererData(List<Cell> visibleCellList, int cameraIndex)
        {
            Profiler.BeginSample("UpdateCameraRendererData");
            for (var protoIndex = 0; protoIndex <= SelectionData.PrototypeList.Count - 1; protoIndex++)
            {
                List<Cell> currentCellList = TerrainObjectRendererData.GetCellsWithInstances(visibleCellList,
                    SelectionData.PrototypeList[protoIndex].ID, out var totalInstanceCount);

                UpdateGPUBuffer(SelectionData, currentCellList, totalInstanceCount, protoIndex, cameraIndex);
            }

            Profiler.EndSample();
        }

        private void UpdateGPUBuffer(SelectionData selectionData, List<Cell> instancedIndirectCellList,
            int totalInstanceCount, int protoIndex, int cameraIndex)
        {
            RendererStackManager rendererStackManager = RendererStackManager.Instance;

            var cameraManager =
                (CameraManager)rendererStackManager.SceneComponentStack.GetElement(typeof(CameraManager));

            VirtualCamera virtualCamera = cameraManager.VirtualCameraList[cameraIndex];
            var objectCameraRender =
                (ObjectCameraRender)virtualCamera.CameraTemporaryComponentStack.GetElement(typeof(ObjectCameraRender));

            var proto = (PrototypeTerrainObject)selectionData.PrototypeList[protoIndex];
            RenderModel renderModel = proto.RenderModel;
            PrototypeRenderData prototypeRenderData = objectCameraRender.PrototypeRenderDataList[protoIndex];

            GPUFrustumCullingID.SetFrustumCullingPlanes(virtualCamera.Camera);

            prototypeRenderData.ClearRenderData();

            if (instancedIndirectCellList.Count == 0 || totalInstanceCount == 0)
            {
                return;
            }

            if (totalInstanceCount > prototypeRenderData.MergeBuffer.ComputeBuffer.count)
            {
                prototypeRenderData.UpdateComputeBufferSize(totalInstanceCount + 5000);
            }

            var threadGroupsFrustum = Mathf.CeilToInt(totalInstanceCount / GPUFrustumCullingID.Numthreads);

            if (threadGroupsFrustum == 0)
            {
                return;
            }

            MergeInstancedIndirectBuffersID.MergeBuffer(instancedIndirectCellList, prototypeRenderData, proto.ID);

            GPUFrustumCullingID.DispatchGPUFrustumCulling(rendererStackManager, prototypeRenderData, proto, renderModel,
                virtualCamera, totalInstanceCount, threadGroupsFrustum);

            for (var lodIndex = 0; lodIndex < renderModel.LODs.Count; lodIndex++)
            for (var n = 0; n < renderModel.LODs[lodIndex].Mesh.subMeshCount; n++)
            {
                ComputeBuffer.CopyCount(prototypeRenderData.LODRenderDataList[lodIndex].PositionBuffer.ComputeBuffer,
                    prototypeRenderData.LODRenderDataList[lodIndex].ArgsBufferMergedLODList[n].ComputeBuffer,
                    sizeof(uint) * 1);
                ComputeBuffer.CopyCount(
                    prototypeRenderData.LODRenderDataList[lodIndex].PositionShadowBuffer.ComputeBuffer,
                    prototypeRenderData.LODRenderDataList[lodIndex].ShadowArgsBufferMergedLODList[n].ComputeBuffer,
                    sizeof(uint) * 1);
            }
        }
    }
}
