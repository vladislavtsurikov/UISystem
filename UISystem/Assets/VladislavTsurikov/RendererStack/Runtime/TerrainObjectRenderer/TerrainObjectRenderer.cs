using UnityEngine.SceneManagement;
using VladislavTsurikov.ReflectionUtility;
using VladislavTsurikov.RendererStack.Editor.Core.PrototypeRendererSystem;
using VladislavTsurikov.RendererStack.Runtime.Common.PrototypeSettings;
using VladislavTsurikov.RendererStack.Runtime.Common.TerrainSystem;
using VladislavTsurikov.RendererStack.Runtime.Core;
using VladislavTsurikov.RendererStack.Runtime.Core.GlobalSettings;
using VladislavTsurikov.RendererStack.Runtime.Core.PrototypeRendererSystem;
using VladislavTsurikov.RendererStack.Runtime.Core.PrototypeRendererSystem.PrototypeSettings;
using VladislavTsurikov.RendererStack.Runtime.Core.RendererSystem;
using VladislavTsurikov.RendererStack.Runtime.Core.SceneSettings;
using VladislavTsurikov.RendererStack.Runtime.Core.SceneSettings.Camera;
using VladislavTsurikov.RendererStack.Runtime.Core.SceneSettings.Camera.CameraSettingsSystem;
using VladislavTsurikov.RendererStack.Runtime.Core.SceneSettings.Camera.CameraTemporarySettingsSystem.Attributes;
using
    VladislavTsurikov.RendererStack.Runtime.Core.SceneSettings.Camera.CameraTemporarySettingsSystem.ObjectCameraRender;
using VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.Data;
using VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.GlobalSettings.ExtensionSystem;
using VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.GPUInstancedIndirect;
using VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.SceneSettings;
using VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.SceneSettings.Camera;
using VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.ScriptingSystem.PrototypeSettings;
using VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.ScriptingSystem.PrototypeSettings.Scripting;
using VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.ScriptingSystem.SceneSettings.Camera;
using VladislavTsurikov.SceneDataSystem.Runtime;
using VladislavTsurikov.SceneDataSystem.Runtime.Utility;
using LODGroup = VladislavTsurikov.RendererStack.Runtime.Core.PrototypeRendererSystem.PrototypeSettings.LODGroup;

namespace VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer
{
    [Name("Terrain Object Renderer")]
    [PrototypeType(typeof(PrototypeTerrainObject))]
    [GenerateGPUInstancedIndirectShaders]
    [AddSceneData(new[] { typeof(TerrainObjectRendererData), typeof(TerrainManager) })]
    [AddCameraTemporaryComponents(new[] { typeof(ObjectCameraRender), typeof(CameraCollidersController) })]
    [AddCameraComponents(new[] { typeof(TerrainObjectRendererCameraSettings) })]
    [AddSceneComponents(new[] { typeof(CameraManager), typeof(Quality) })]
    [AddGlobalComponents(new[] { typeof(Common.GlobalSettings.Quality), typeof(ExtensionSystem) })]
    [AddPrototypeComponents(new[]
    {
        typeof(LODGroup), typeof(Shadow), typeof(DistanceCulling), typeof(FrustumCulling), typeof(Colliders),
        typeof(Scripting)
    })]
    public partial class TerrainObjectRenderer : PrototypeRenderer
    {
        public delegate void SetupPrototypeRendererRendererAfter();

        private static TerrainObjectRenderer s_instance;
        public static SetupPrototypeRendererRendererAfter AfterSetupPrototypeRendererRendererEvent;
        public bool DebugAllCells = false;
        public bool DebugVisibleCells = false;

        public ScriptingSystem.ScriptingSystem ScriptingSystem;

        public static TerrainObjectRenderer Instance
        {
            get
            {
                if (RendererStackManager.Instance == null)
                {
                    return null;
                }

                s_instance =
                    (TerrainObjectRenderer)RendererStackManager.Instance.RendererStack.GetElement(
                        typeof(TerrainObjectRenderer));

                if (s_instance == null)
                {
                    s_instance =
                        (TerrainObjectRenderer)RendererStackManager.Instance.RendererStack.CreateIfMissingType(
                            typeof(TerrainObjectRenderer));
                }

                return s_instance;
            }
        }

        protected override void SetupPrototypeRendererRenderer()
        {
            if (ScriptingSystem == null)
            {
                ScriptingSystem = new ScriptingSystem.ScriptingSystem();
            }

            MergeInstancedIndirectBuffersID.Setup();
            GPUFrustumCullingID.Setup();

            ScriptingSystem.Setup();

            TerrainManager.ChangedTerrainCountEvent -= NewTerrainAction;
            TerrainManager.ChangedTerrainCountEvent += NewTerrainAction;

            AfterSetupPrototypeRendererRendererEvent?.Invoke();
        }

        protected override void OnDisableElement()
        {
            ScriptingSystem?.OnDisable();
            MergeInstancedIndirectBuffersID.Dispose();
            GPUFrustumCullingID.Dispose();
        }

        private void NewTerrainAction(SceneDataManager sceneDataManager)
        {
            if (SceneManager.sceneCount == 1)
            {
                TerrainObjectRendererData.RefreshAllCells();
            }
        }

        public override void DrawDebug()
        {
            foreach (TerrainObjectRendererData item in
                     SceneDataStackUtility.GetAllSceneData<TerrainObjectRendererData>())
            {
                item.DrawDebug();
            }
        }
    }
}
