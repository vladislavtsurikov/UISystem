#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.Rendering;

namespace VladislavTsurikov.RendererStack.Runtime.Core.SceneSettings.Camera
{
    public partial class CameraManager
    {
        private void StartEditorSimulation()
        {
            if (Application.isPlaying)
            {
                return;
            }

            RenderPipelineManager.beginCameraRendering -= BeginRenderingSrp;
            RenderPipelineManager.beginCameraRendering += BeginRenderingSrp;

            UnityEngine.Camera.onPreCull -= BeginRendering;
            UnityEngine.Camera.onPreCull += BeginRendering;
        }

        private void StopEditorSimulation()
        {
            RenderPipelineManager.beginCameraRendering -= BeginRenderingSrp;
            UnityEngine.Camera.onPreCull -= BeginRendering;
        }

        private void BeginRendering(UnityEngine.Camera cam)
        {
            if (RendererStackManager.Instance == null || RendererStackManager.Instance.EditorPlayModeSimulation)
            {
                return;
            }

            VirtualCamera camera = GetSceneCamera();

            if (camera != null)
            {
                if (cam.name == "SceneCamera")
                {
                    camera.Camera = cam;
                }

                RendererStackManager.Instance.RendererStack.Render();
            }
        }

        private void BeginRenderingSrp(ScriptableRenderContext context, UnityEngine.Camera cam)
        {
            if (RendererStackManager.Instance.EditorPlayModeSimulation)
            {
                return;
            }

            VirtualCamera camera = GetSceneCamera();

            if (camera != null)
            {
                if (cam.name == "SceneCamera")
                {
                    camera.Camera = cam;
                }

                RendererStackManager.Instance.RendererStack.Render();
            }
        }
    }
}
#endif
