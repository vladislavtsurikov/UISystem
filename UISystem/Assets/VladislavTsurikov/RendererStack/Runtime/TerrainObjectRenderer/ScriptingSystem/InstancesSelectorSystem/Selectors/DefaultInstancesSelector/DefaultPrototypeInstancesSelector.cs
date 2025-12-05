using System;
using System.Collections.Generic;
using UnityEngine.Profiling;
using VladislavTsurikov.ColliderSystem.Runtime;
using VladislavTsurikov.Math.Runtime;
using VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.Data;
using VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.Data.ColliderSystem;
using Object = System.Object;

namespace VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.ScriptingSystem.InstancesSelectorSystem.
    DefaultInstancesSelector
{
    public class DefaultPrototypeInstancesSelector : PrototypeInstancesSelector
    {
        private readonly ScriptingSystemPool _scriptingSystemPool;

        public DefaultPrototypeInstancesSelector(PrototypeScriptingManager prototypeScriptingManager,
            PrototypeTerrainObject proto) : base(proto) =>
            _scriptingSystemPool = new ScriptingSystemPool(prototypeScriptingManager);

        public override void SetColliders(Sphere sphere, Object usedObj)
        {
            var defaultPrototypeInstancesSelectorData =
                (DefaultPrototypeInstancesSelectorData)GetPrototypeInstancesSelectorData(usedObj);

            Profiler.BeginSample("ColliderUtility.OverlapSphere");

            List<ColliderObject> newInstanceList = TerrainObjectRendererAPI.OverlapSphere(sphere.Center, sphere.Radius,
                ObjectFilter, true);

            Profiler.EndSample();

            Profiler.BeginSample("After finding objects");

            foreach (ColliderObject colliderObject in newInstanceList)
            {
                var objectCollider = (TerrainObjectCollider)colliderObject;

                if (objectCollider.Instance.HierarchyTerrainObjectInstance == null)
                {
                    EnableCollider(objectCollider, usedObj);
                    continue;
                }

                if (defaultPrototypeInstancesSelectorData.LastDistanceCullingSphere.IsValid)
                {
                    if (!defaultPrototypeInstancesSelectorData.LastDistanceCullingSphere.Contains(objectCollider
                            .Instance.Position))
                    {
                        EnableCollider(objectCollider, usedObj);
                    }
                    else
                    {
                        defaultPrototypeInstancesSelectorData.LastInstanceList.Remove(objectCollider);
                    }
                }
                else
                {
                    EnableCollider(objectCollider, usedObj);
                }
            }

            foreach (ColliderObject colliderObject in defaultPrototypeInstancesSelectorData.LastInstanceList)
            {
                var terrainObjectCollider = (TerrainObjectCollider)colliderObject;
                DisableCollider(terrainObjectCollider, usedObj);
            }

            defaultPrototypeInstancesSelectorData.LastDistanceCullingSphere = sphere;
            defaultPrototypeInstancesSelectorData.LastInstanceList.Clear();
            defaultPrototypeInstancesSelectorData.LastInstanceList = newInstanceList;

            Profiler.EndSample();
        }

        public override void OnNewInstanceVisibility(TerrainObjectInstance instance) =>
            _scriptingSystemPool.Get(instance);

        public override void OnInstanceInvisible(TerrainObjectInstance instance)
        {
            if (instance.HierarchyTerrainObjectInstance == null)
            {
                return;
            }

            _scriptingSystemPool.Release(instance.HierarchyTerrainObjectInstance.gameObject);
            instance.HierarchyTerrainObjectInstance = null;
        }

        protected override void OnDisableCollider(PrototypeInstancesSelectorData prototypeInstancesSelectorData,
            object usedObj)
        {
            var defaultPrototypeInstancesSelectorData =
                (DefaultPrototypeInstancesSelectorData)prototypeInstancesSelectorData;

            foreach (ColliderObject colliderObject in defaultPrototypeInstancesSelectorData.LastInstanceList)
            {
                var terrainObjectCollider = (TerrainObjectCollider)colliderObject;

                DisableCollider(terrainObjectCollider, usedObj);
            }
        }

        protected override void OnDisableCollider(PrototypeInstancesSelectorData prototypeInstancesSelectorData)
        {
            var defaultPrototypeInstancesSelectorData =
                (DefaultPrototypeInstancesSelectorData)prototypeInstancesSelectorData;

            foreach (ColliderObject colliderObject in defaultPrototypeInstancesSelectorData.LastInstanceList)
            {
                var terrainObjectCollider = (TerrainObjectCollider)colliderObject;

                DisableCollider(terrainObjectCollider);
            }
        }

        public override Type GetInstanceSelectorData() => typeof(DefaultPrototypeInstancesSelectorData);
    }
}
