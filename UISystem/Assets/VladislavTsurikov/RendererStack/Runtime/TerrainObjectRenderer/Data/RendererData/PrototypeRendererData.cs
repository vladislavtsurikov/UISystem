using System;
using System.Collections.Generic;
using OdinSerializer;
#if BillboardSystem
using VladislavTsurikov.InstantRenderer.LargeObjectRenderer.Scripts.RendererData.BillboardSystem;
#endif

namespace VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.Data.RendererData
{
    [Serializable]
    public class PrototypeRendererData
    {
        [OdinSerialize]
        private int _prototypeID;

        [NonSerialized]
        public TemporaryInstances TemporaryInstances = new();

#if BillboardSystem
        [NonSerialized]
        public CombinedImposterMesh CombinedImposterMesh = new CombinedImposterMesh();
#endif

        public int PrototypeID => _prototypeID;

        [OdinSerialize]
        public List<Instance> InstanceList = new();

        public PrototypeRendererData(int prototypeID) => _prototypeID = prototypeID;

        public void Setup()
        {
            TemporaryInstances = new TemporaryInstances();
#if BillboardSystem
            CombinedImposterMesh = new CombinedImposterMesh();
#endif
            ConvertPersistentDataToTemporaryData();
        }

        public void ConvertPersistentDataToTemporaryData()
        {
            if (TemporaryInstances == null)
            {
                TemporaryInstances = new TemporaryInstances();
            }

            TemporaryInstances.ConvertPersistentDataToTemporaryData(InstanceList);
#if BillboardSystem
            CombinedImposterMesh.CreateImposterMesh(PrototypeID, this);
#endif
        }

        public void AddPersistentData(TerrainObjectInstance instance)
        {
            Instance item;
            item.ID = instance.ID;
            item.Position = instance.Position;
            item.Rotation = instance.Rotation;
            item.Scale = instance.Scale;
            InstanceList.Add(item);

            ModifiedPrototypeRenderDataStack.Add(this);
        }

        public void ClearPersistentData()
        {
            if (InstanceList.Count == 0)
            {
                return;
            }

            InstanceList.Clear();
            ModifiedPrototypeRenderDataStack.Add(this);
        }

        public void RemovePersistentInstance(TerrainObjectInstance instance)
        {
            for (var i = 0; i < InstanceList.Count; i++)
            {
                if (InstanceList[i].ID == instance.ID)
                {
                    InstanceList.RemoveAt(i);
                    ModifiedPrototypeRenderDataStack.Add(this);
                    return;
                }
            }
        }

        public void SyncPersistentRendererInstance(TerrainObjectInstance instance)
        {
            for (var i = 0; i < InstanceList.Count; i++)
            {
                if (InstanceList[i].ID == instance.ID)
                {
                    InstanceList[i] = Instance.ConvertToSerializableInstance(instance);
                    ModifiedPrototypeRenderDataStack.Add(this);
                    return;
                }
            }
        }

        public void SetEnable(TerrainObjectInstance instance)
        {
            for (var i = 0; i < InstanceList.Count; i++)
            {
                if (InstanceList[i].ID == instance.ID)
                {
                    if (instance.Enable)
                    {
                        InstanceList.Add(Instance.ConvertToSerializableInstance(instance));
                    }
                    else
                    {
                        InstanceList.RemoveAt(i);
                    }

                    ModifiedPrototypeRenderDataStack.Add(this);
                    return;
                }
            }
        }

        public void OnDisable()
        {
            TemporaryInstances?.DisposeUnmanagedMemory();

#if BillboardSystem
            CombinedImposterMesh?.Dispose();
#endif
        }
    }
}
