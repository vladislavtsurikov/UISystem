using System;
using System.Runtime.Serialization;
using System.Threading;
using OdinSerializer;
using UnityEngine;
using VladislavTsurikov.AttributeUtility.Runtime;
using VladislavTsurikov.ComponentStack.Runtime.AdvancedComponentStack;
using VladislavTsurikov.RendererStack.Runtime.Common.GlobalSettings;
using VladislavTsurikov.RendererStack.Runtime.Core.Preferences;
using VladislavTsurikov.RendererStack.Runtime.Core.SceneSettings.Camera.CameraSettingsSystem;
using VladislavTsurikov.RendererStack.Runtime.Core.SceneSettings.Camera.CameraTemporarySettingsSystem;
using VladislavTsurikov.RendererStack.Runtime.Core.SceneSettings.Camera.CameraTemporarySettingsSystem.Attributes;
using Renderer = VladislavTsurikov.RendererStack.Runtime.Core.RendererSystem.Renderer;
#if UNITY_EDITOR
using VladislavTsurikov.RendererStack.Editor.Core.SceneSettings.Camera.CameraSettingsSystem;
#endif

namespace VladislavTsurikov.RendererStack.Runtime.Core.SceneSettings.Camera
{
    public enum CameraType
    {
        Normal,
        SceneView
    }

    [Serializable]
    public class VirtualCamera
    {
        public UnityEngine.Camera Camera;
        public bool Active = true;

        public CameraType CameraType = CameraType.Normal;
        public Vector3 FloatingOriginOffset = new(0, 0, 0);

        public bool FoldoutGUI = true;

        [NonSerialized]
        private Matrix4x4 _lastCameraTransform;

        [NonSerialized]
        private float _lastFieldOfView;

        [NonSerialized]
        private float _lastMaxDistance;

        [OdinSerialize]
        public ComponentStackOnlyDifferentTypes<CameraComponent> CameraComponentStack = new();

#if UNITY_EDITOR
        public CameraComponentStackEditor CameraComponentStackEditor;
#endif

        [NonSerialized]
        public ComponentStackOnlyDifferentTypes<CameraTemporaryComponent> CameraTemporaryComponentStack;

        public VirtualCamera(UnityEngine.Camera selectedCamera)
        {
            Camera = selectedCamera;
            Setup();
        }

        public bool Ignored => !IsCameraActive();

        [OnDeserializing]
        private void OnDeserializing() =>
            CameraComponentStack ??= new ComponentStackOnlyDifferentTypes<CameraComponent>();

        public void Setup()
        {
            OnDisable();
            CameraTemporaryComponentStack = new ComponentStackOnlyDifferentTypes<CameraTemporaryComponent>();
            CameraTemporaryComponentStack.Setup(true, new object[] { this });

            CameraComponentStack.Setup();

            foreach (Renderer renderer in RendererStackManager.Instance.RendererStack.ElementList)
            {
                var addCameraTemporaryComponentsAttribute =
                    (AddCameraTemporaryComponentsAttribute)renderer.GetType()
                        .GetAttribute(typeof(AddCameraTemporaryComponentsAttribute));
                var addCameraComponentsAttribute =
                    (AddCameraComponentsAttribute)renderer.GetType().GetAttribute(typeof(AddCameraComponentsAttribute));

                if (addCameraTemporaryComponentsAttribute != null)
                {
                    CameraTemporaryComponentStack.CreateIfMissingType(addCameraTemporaryComponentsAttribute.Types);
                }

                if (addCameraComponentsAttribute != null)
                {
                    CameraComponentStack.CreateIfMissingType(addCameraComponentsAttribute.Types);
                }
            }

#if UNITY_EDITOR
            CameraComponentStackEditor = new CameraComponentStackEditor(CameraComponentStack);
#endif
        }

        public UnityEngine.Camera GetRenderingCamera()
        {
#if UNITY_EDITOR
            if (Application.isPlaying == false)
            {
                if (RendererStackManager.Instance.EditorPlayModeSimulation)
                {
                    return null;
                }
            }
#endif
            if (RendererStackSettings.Instance.RenderDirectToCamera)
            {
                if (CameraType == CameraType.SceneView || Application.isPlaying)
                {
                    return Camera;
                }
            }
            else
            {
                if (CameraType == CameraType.SceneView)
                {
                    return Camera;
                }
            }

            return null;
        }

        public bool IsCameraActive()
        {
#if UNITY_EDITOR
            var fitCamera = IsFitCamera();

            if (!fitCamera)
            {
                return false;
            }

            if (Application.isPlaying == false)
            {
                if (RendererStackManager.Instance.EditorPlayModeSimulation)
                {
                    if (CameraType == CameraType.SceneView)
                    {
                        return false;
                    }
                }
                else
                {
                    if (CameraType == CameraType.Normal)
                    {
                        return false;
                    }
                }

                return true;
            }

            if (CameraType == CameraType.SceneView)
            {
                if (RendererStackSettings.Instance.RenderSceneCameraInPlayMode)
                {
                    return true;
                }

                return false;
            }

            return true;
#else
            return IsFitCamera();
#endif
        }

        public bool IsFitCamera()
        {
            if (CameraType == CameraType.Normal)
            {
                if (Camera == null || Camera.gameObject.activeInHierarchy == false || Camera.enabled == false)
                {
                    return false;
                }

                return Active;
            }

            return Camera != null;
        }

        public Vector3 GetCameraPosition() => Camera.transform.position - FloatingOriginOffset;

        public float GetMaxDistance(Type rendererType)
        {
            var quality = (Quality)GlobalSettings.GlobalSettings.Instance.GetElement(typeof(Quality), rendererType);

            var maxDistance = Mathf.Min(Camera.farClipPlane, quality.MaxRenderDistance);

            var fieldOfViewFactor = 90 / Mathf.Clamp(Camera.fieldOfView, 0, 90);

            maxDistance *= fieldOfViewFactor;

            return maxDistance;
        }

        public Matrix4x4 GetMatrix4X4()
        {
            Transform transform = Camera.transform;
            return Matrix4x4.TRS(transform.position, transform.rotation, transform.localScale);
        }

        public bool IsPositionChanged()
        {
            var needUpdate = !_lastCameraTransform.GetPosition().Equals(Camera.transform.position);

            _lastCameraTransform = GetMatrix4X4();

            return needUpdate;
        }

        public bool IsNeedUpdateRenderData(Type rendererType)
        {
            var needUpdate = _lastMaxDistance != GetMaxDistance(rendererType)
                             || !_lastCameraTransform.Equals(GetMatrix4X4())
                             || _lastFieldOfView != Camera.fieldOfView;

            return needUpdate;
        }

        public void RefreshCameraData(Type rendererType)
        {
            _lastMaxDistance = GetMaxDistance(rendererType);
            _lastCameraTransform = GetMatrix4X4();
            _lastFieldOfView = Camera.fieldOfView;
        }

        public void OnDisable() => CameraTemporaryComponentStack?.OnDisable();
    }
}
