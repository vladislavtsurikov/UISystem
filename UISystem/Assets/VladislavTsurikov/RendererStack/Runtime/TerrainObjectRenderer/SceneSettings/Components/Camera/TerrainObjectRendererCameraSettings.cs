using System;
using VladislavTsurikov.RendererStack.Runtime.Core.SceneSettings.Camera.CameraSettingsSystem;

namespace VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.SceneSettings.Camera
{
    [Serializable]
    public enum CameraCullingMode
    {
        FrustumCulling = 0,
        Complete360 = 1
    }

    public class TerrainObjectRendererCameraSettings : CameraComponent
    {
        public CameraCullingMode CameraCullingMode = CameraCullingMode.FrustumCulling;
        public bool EnableColliders = true;
        public float LodBias = 1;
    }
}
