using UnityEngine;
using VladislavTsurikov.ColliderSystem.Runtime;
using VladislavTsurikov.MegaWorld.Runtime.Common.Area;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings;
using VladislavTsurikov.MegaWorld.Runtime.Common.Stamper;
using VladislavTsurikov.MegaWorld.Runtime.Core.GlobalSettings.ElementsSystem;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group;
using VladislavTsurikov.MegaWorld.Runtime.Core.Utility;
using VladislavTsurikov.MegaWorld.Runtime.GravitySpawner.Utility;
using VladislavTsurikov.PhysicsSimulator.Runtime;

namespace VladislavTsurikov.MegaWorld.Runtime.GravitySpawner
{
    public class GravitySpawnerGameObject : OnDisableSimulatedBodyEvent
    {
        private readonly BoxArea _area;
        private readonly Group _group;
        private readonly TerrainsMaskManager _terrainsMaskManager;

        public GravitySpawnerGameObject(Group group, TerrainsMaskManager terrainsMaskManager, BoxArea area)
        {
            _group = group;
            _terrainsMaskManager = terrainsMaskManager;
            _area = area;
        }

        protected override void OnDisablePhysics()
        {
            Vector3 position = SimulatedBody.GameObject.transform.position;

            if (!_area.Contains(position))
            {
                Object.DestroyImmediate(SimulatedBody.GameObject);
                return;
            }

            RayHit rayHit =
                RaycastUtility.Raycast(RayUtility.GetRayDown(new Vector3(position.x, _area.Center.y, position.z)),
                    GlobalCommonComponentSingleton<LayerSettings>.Instance.GetCurrentPaintLayers(_group.PrototypeType));
            if (rayHit != null)
            {
                var fitness = GetFitness.Get(_group, _terrainsMaskManager, rayHit);

                if (Random.Range(0f, 1f) < 1 - fitness)
                {
                    Object.DestroyImmediate(SimulatedBody.GameObject);
                }
            }
        }
    }
}
