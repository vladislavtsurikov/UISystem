using System;
using VladislavTsurikov.RendererStack.Runtime.Core.SceneSettings.Camera.CameraTemporarySettingsSystem;

namespace VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.ScriptingSystem.SceneSettings.Camera
{
    public class CameraCollidersController : CameraTemporaryComponent
    {
        [NonSerialized]
        public bool UsedForColliders = false;

        protected override void OnDisableElement() => ScriptingSystem.RemoveColliders(VirtualCamera);
    }
}
