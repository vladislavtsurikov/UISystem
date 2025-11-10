using VladislavTsurikov.ComponentStack.Runtime.Core;

namespace VladislavTsurikov.RendererStack.Runtime.Core.SceneSettings.Camera.CameraTemporarySettingsSystem
{
    public class CameraTemporaryComponent : Component
    {
        protected VirtualCamera VirtualCamera;

        protected override void SetupComponent(object[] setupData = null)
        {
            VirtualCamera = (VirtualCamera)setupData[0];

            SetupCameraTemporaryComponent();
        }

        protected virtual void SetupCameraTemporaryComponent()
        {
        }
    }
}
