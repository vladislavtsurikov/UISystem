using System;
using System.Collections.Generic;
using UnityEngine;
using VladislavTsurikov.ColliderSystem.Runtime;
using VladislavTsurikov.Math.Runtime;
using VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.Data;
using VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.Data.ColliderSystem;
using Object = System.Object;

namespace VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.ScriptingSystem.InstancesSelectorSystem
{
    public abstract class PrototypeInstancesSelector
    {
        private readonly List<PrototypeInstancesSelectorData> _prototypeInstancesSelectorDataList = new();
        protected readonly ObjectFilter ObjectFilter;
        protected readonly PrototypeTerrainObject PrototypeTerrainObject;

        protected PrototypeInstancesSelector(PrototypeTerrainObject proto)
        {
            PrototypeTerrainObject = proto;

            ObjectFilter = new ObjectFilter();
            ObjectFilter.SetFindPrefabs(new List<GameObject> { PrototypeTerrainObject.Prefab });
        }

        public abstract void SetColliders(Sphere sphere, Object usedObj);
        public abstract void OnNewInstanceVisibility(TerrainObjectInstance terrainObjectInstance);
        public abstract void OnInstanceInvisible(TerrainObjectInstance terrainObjectInstance);

        protected abstract void OnDisableCollider(PrototypeInstancesSelectorData prototypeInstancesSelectorData,
            object usedObj);

        protected abstract void OnDisableCollider(PrototypeInstancesSelectorData prototypeInstancesSelectorData);
        public abstract Type GetInstanceSelectorData();

        protected void EnableCollider(TerrainObjectCollider terrainObjectCollider, Object usedObj)
        {
            if (terrainObjectCollider.Instance.HierarchyTerrainObjectInstance == null)
            {
                OnNewInstanceVisibility(terrainObjectCollider.Instance);
            }

            if (!terrainObjectCollider.Instance.HierarchyTerrainObjectInstance.UsedObjects.Contains(usedObj))
            {
                terrainObjectCollider.Instance.HierarchyTerrainObjectInstance.UsedObjects.Add(usedObj);
            }
        }

        protected void DisableCollider(TerrainObjectCollider terrainObjectCollider, Object usedObj)
        {
            if (terrainObjectCollider.Instance.HierarchyTerrainObjectInstance == null)
            {
                return;
            }

            terrainObjectCollider.Instance.HierarchyTerrainObjectInstance.UsedObjects.Remove(usedObj);

            if (terrainObjectCollider.Instance.HierarchyTerrainObjectInstance.UsedObjects.Count == 0)
            {
                OnInstanceInvisible(terrainObjectCollider.Instance);
            }
        }

        protected void DisableCollider(TerrainObjectCollider terrainObjectCollider)
        {
            if (terrainObjectCollider.Instance.HierarchyTerrainObjectInstance == null)
            {
                return;
            }

            OnInstanceInvisible(terrainObjectCollider.Instance);
        }

        public void OnDisableCollider(Object usedObj)
        {
            for (var i = _prototypeInstancesSelectorDataList.Count - 1; i >= 0; i--)
            {
                if (_prototypeInstancesSelectorDataList[i].Object == usedObj)
                {
                    OnDisableCollider(_prototypeInstancesSelectorDataList[i], usedObj);
                    _prototypeInstancesSelectorDataList.RemoveAt(i);
                }
            }
        }

        public void OnDisableCollider()
        {
            for (var i = _prototypeInstancesSelectorDataList.Count - 1; i >= 0; i--)
            {
                OnDisableCollider(_prototypeInstancesSelectorDataList[i]);
                _prototypeInstancesSelectorDataList.RemoveAt(i);
            }
        }

        public PrototypeInstancesSelectorData GetPrototypeInstancesSelectorData(Object usedObj)
        {
            foreach (PrototypeInstancesSelectorData item in _prototypeInstancesSelectorDataList)
            {
                if (item.Object == usedObj)
                {
                    return item;
                }
            }

            var prototypeInstancesSelectorData =
                (PrototypeInstancesSelectorData)Activator.CreateInstance(GetInstanceSelectorData(), usedObj);

            _prototypeInstancesSelectorDataList.Add(prototypeInstancesSelectorData);

            return prototypeInstancesSelectorData;
        }
    }
}
