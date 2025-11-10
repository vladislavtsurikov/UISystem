using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.ReflectionUtility;

namespace VladislavTsurikov.RendererStack.Runtime.Core.SceneSettings.Camera
{
    [Name("Cameras")]
    public partial class CameraManager : SceneComponent
    {
        public List<VirtualCamera> VirtualCameraList = new();

        [OnDeserializing]
        private void OnDeserializing() => VirtualCameraList ??= new List<VirtualCamera>();

        protected override void SetupComponent(object[] setupData = null)
        {
            for (var i = VirtualCameraList.Count - 1; i >= 0; i--)
            {
                if (VirtualCameraList[i].CameraType == CameraType.SceneView || VirtualCameraList[i].Camera == null)
                {
                    RemoveCamera(i);
                }
            }

            foreach (VirtualCamera cam in VirtualCameraList)
            {
                cam.Setup();
            }

#if UNITY_EDITOR
            StartFindSceneViewCamera();

            if (!Application.isPlaying)
            {
                StartEditorSimulation();
            }
#endif

        }

        protected override void OnCreate() => FindMainCamera();

        protected override void OnDisableElement()
        {
#if UNITY_EDITOR
            EditorApplication.update -= FindSceneViewCamera;

            if (!Application.isPlaying)
            {
                StopEditorSimulation();
            }
#endif

            for (var i = 0; i < VirtualCameraList.Count; i++)
            {
                VirtualCameraList[i].OnDisable();
            }
        }

        public void FindMainCamera()
        {
            UnityEngine.Camera selectedCamera = UnityEngine.Camera.main;

            if (selectedCamera == null)
            {
#if UNITY_6000_0_OR_NEWER
                UnityEngine.Camera[] cameras = Object.FindObjectsByType<UnityEngine.Camera>(FindObjectsSortMode.None);
#else
                UnityEngine.Camera[] cameras = Object.FindObjectsOfType<UnityEngine.Camera>();
#endif
                for (var i = 0; i <= cameras.Length - 1; i++)
                {
                    if (cameras[i].gameObject.name.Contains("Main Camera") ||
                        cameras[i].gameObject.name.Contains("MainCamera"))
                    {
                        selectedCamera = cameras[i];
                        break;
                    }
                }
            }

            AddCamera(selectedCamera);
        }

        public void FindAllCamera()
        {
#if UNITY_6000_0_OR_NEWER
            UnityEngine.Camera[] cameras = Object.FindObjectsByType<UnityEngine.Camera>(FindObjectsSortMode.None);
#else
            UnityEngine.Camera[] cameras = Object.FindObjectsOfType<UnityEngine.Camera>();
#endif
            for (var i = 0; i <= cameras.Length - 1; i++)
            {
                AddCamera(cameras[i]);
            }
        }

        public void AddCamera(UnityEngine.Camera camera)
        {
            VirtualCamera virtualCamera = GetVirtualCamera(camera);
            if (virtualCamera == null)
            {
                virtualCamera = new VirtualCamera(camera);

                AddCamera(virtualCamera);
            }
        }

        private void AddCamera(VirtualCamera virtualCamera)
        {
            if (GetVirtualCamera(virtualCamera.Camera) == null)
            {
                VirtualCameraList.Add(virtualCamera);
            }
        }

        public void RemoveCamera(int index)
        {
            VirtualCamera virtualCamera = VirtualCameraList[index];
            if (virtualCamera != null)
            {
                VirtualCameraList.RemoveAt(index);
                virtualCamera.CameraTemporaryComponentStack?.OnDisable();
            }
        }

        public void RemoveCamera(UnityEngine.Camera camera)
        {
            VirtualCamera virtualCamera = GetVirtualCamera(camera);
            if (virtualCamera != null)
            {
                VirtualCameraList.Remove(virtualCamera);
                virtualCamera.CameraTemporaryComponentStack?.OnDisable();
            }
        }

        public VirtualCamera GetVirtualCamera(UnityEngine.Camera unityCamera)
        {
            foreach (VirtualCamera cam in VirtualCameraList)
            {
                if (cam.Camera == unityCamera)
                {
                    return cam;
                }
            }

            return null;
        }

        public VirtualCamera GetSceneCamera()
        {
            foreach (VirtualCamera cam in VirtualCameraList)
            {
                if (cam.CameraType == CameraType.SceneView)
                {
                    return cam;
                }
            }

            return null;
        }

        public bool IsMultipleCameras()
        {
            if (Application.isPlaying)
            {
                return VirtualCameraList.Count > 1;
            }

            return VirtualCameraList.Count > 2;
        }

#if UNITY_EDITOR
        private void StartFindSceneViewCamera()
        {
            EditorApplication.update -= FindSceneViewCamera;
            EditorApplication.update += FindSceneViewCamera;
        }

        private void FindSceneViewCamera()
        {
            UnityEngine.Camera currentCam = UnityEngine.Camera.current;
            if (currentCam != null && currentCam.name == "SceneCamera")
            {
                VirtualCamera sceneViewCameraData = GetSceneCamera();

                if (sceneViewCameraData == null)
                {
                    var sceneviewCamera =
                        new VirtualCamera(null) { CameraType = CameraType.SceneView };

                    AddCamera(sceneviewCamera);
                    sceneViewCameraData = sceneviewCamera;
                }

                if (sceneViewCameraData.Camera == null)
                {
                    sceneViewCameraData.Camera = currentCam;
                }
            }
        }
#endif
    }
}
