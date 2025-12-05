#if RENDERER_STACK
using Cysharp.Threading.Tasks;
using UnityEngine;
using VladislavTsurikov.ColliderSystem.Runtime;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings;
using VladislavTsurikov.MegaWorld.Runtime.Core.GlobalSettings.ElementsSystem;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group;
using VladislavTsurikov.MegaWorld.Runtime.Core.Utility;
using VladislavTsurikov.PhysicsSimulator.Runtime;
using VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer;
using VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.Data;
using VladislavTsurikov.UnityUtility.Runtime;

namespace VladislavTsurikov.MegaWorld.Runtime.Common.PhysXPainter
{
    public class TerrainObjectOnDisablePhysics : OnDisableSimulatedBodyEvent
    {
        protected readonly Group _group;
        protected readonly PrototypeTerrainObject _proto;

        public TerrainObjectOnDisablePhysics(Group group, PrototypeTerrainObject proto)
        {
            _group = group;
            _proto = proto;
        }

        protected override void OnDisablePhysics()
        {
            if (!IsValid())
            {
                return;
            }

            Vector3 position = SimulatedBody.GameObject.transform.position;

            var addedInstance = false;

            RayHit rayHit =
                RaycastUtility.Raycast(RayUtility.GetRayDown(new Vector3(position.x, position.y + 1, position.z)),
                    GlobalCommonComponentSingleton<LayerSettings>.Instance.GetCurrentPaintLayers(_group.PrototypeType));

            if (rayHit != null)
            {
                var fitness = GetFitness(rayHit);

                if (fitness != 0)
                {
                    if (Random.Range(0f, 1f) < 1 - fitness)
                    {
                        Object.DestroyImmediate(SimulatedBody.GameObject);
                        return;
                    }

                    TerrainObjectInstance terrainObjectInstance = TerrainObjectRendererAPI.AddInstance(_proto,
                        SimulatedBody.GameObject.transform.position, SimulatedBody.GameObject.transform.lossyScale,
                        SimulatedBody.GameObject.transform.rotation);

                    var terrainObjectSimulatedBody = (TerrainObjectSimulatedBody)SimulatedBody;
                    terrainObjectSimulatedBody.TerrainObjectInstance = terrainObjectInstance;

                    addedInstance = true;
                }
            }

            if (addedInstance)
            {
                SimulatedBody.GameObject.DisableMeshRenderers();

                DestroyGameObject().Forget();
            }
            else
            {
                Object.DestroyImmediate(SimulatedBody.GameObject);
            }
        }

        private async UniTask DestroyGameObject()
        {
            await UniTask.WaitWhile(() => SimulatedBodyStack.Count > 0);

            Object.DestroyImmediate(SimulatedBody.GameObject);
        }

        protected virtual float GetFitness(RayHit rayHit) => 1;

        protected virtual bool IsValid() => true;
    }
}
#endif
