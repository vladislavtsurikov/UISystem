#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.Data;
using VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.Data.ColliderSystem;
using VladislavTsurikov.SceneDataSystem.Runtime.StreamingUtility;
using VladislavTsurikov.SceneDataSystem.Runtime.Utility;

namespace VladislavTsurikov.RendererStack.Editor.TerrainObjectRenderer
{
    [InitializeOnLoad]
    public static class TerrainObjectRendererStreamingSupport
    {
        private static List<TerrainObjectCollider> _largeObjectInstances;

        static TerrainObjectRendererStreamingSupport()
        {
            Runtime.TerrainObjectRenderer.TerrainObjectRenderer.AfterSetupPrototypeRendererRendererEvent -=
                SetupPrototypeRendererRenderer;
            Runtime.TerrainObjectRenderer.TerrainObjectRenderer.AfterSetupPrototypeRendererRendererEvent +=
                SetupPrototypeRendererRenderer;

            StreamingUtilityEvents.BeforeDeleteAllAdditiveScenesEvent -= ScenesCreatedOrDeleted;
            StreamingUtilityEvents.BeforeDeleteAllAdditiveScenesEvent += ScenesCreatedOrDeleted;

            Runtime.Sectorize.Sectorize.CreateScenesAfterEvent -= ScenesCreatedOrDeleted;
            Runtime.Sectorize.Sectorize.CreateScenesAfterEvent += ScenesCreatedOrDeleted;
        }

        private static void ScenesCreatedOrDeleted()
        {
            _largeObjectInstances = new List<TerrainObjectCollider>();

            foreach (TerrainObjectRendererData item in
                     SceneDataStackUtility.GetAllSceneData<TerrainObjectRendererData>())
            {
                _largeObjectInstances.AddRange(item.GetAllInstances());
            }
        }

        private static void SetupPrototypeRendererRenderer()
        {
            if (_largeObjectInstances != null)
            {
                TerrainObjectRendererData.AddInstances(_largeObjectInstances,
                    Runtime.Sectorize.Sectorize.GetSectorLayerTag());
                _largeObjectInstances.Clear();
                _largeObjectInstances = null;
            }
        }
    }
}
#endif
