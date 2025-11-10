using System;
using System.Collections.Generic;
using OdinSerializer;
using VladislavTsurikov.RendererStack.Runtime.Core.PrototypeRendererSystem;

namespace VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.Data.RendererData
{
    [Serializable]
    public class PrototypeRenderDataStack
    {
        [OdinSerialize]
        public List<PrototypeRendererData> PrototypeRenderDataList = new();

        public void Setup(SelectionData selectionData)
        {
            PreparePrototypeRenderCount(selectionData);

            foreach (PrototypeRendererData prototypeRenderData in PrototypeRenderDataList)
            {
                prototypeRenderData.Setup();
            }
        }

        public void PreparePrototypeRenderCount(SelectionData selectionData)
        {
            for (var i = selectionData.PrototypeList.Count - 1; i >= 0; i--)
            {
                if (selectionData.PrototypeList[i] == null)
                {
                    continue;
                }

                PrototypeRendererData prototypeRendererData = GetPrototypeRenderData(selectionData.PrototypeList[i].ID);

                if (prototypeRendererData == null)
                {
                    PrototypeRenderDataList.Add(new PrototypeRendererData(selectionData.PrototypeList[i].ID));
                }
            }

            for (var i = PrototypeRenderDataList.Count - 1; i >= 0; i--)
            {
                if (selectionData.GetProto(PrototypeRenderDataList[i].PrototypeID) == null)
                {
                    PrototypeRenderDataList.RemoveAt(i);
                }
                else if (PrototypeRenderDataList[i].InstanceList.Count == 0)
                {
                    PrototypeRenderDataList.RemoveAt(i);
                }
            }
        }

        public void RemoveInstances(int id)
        {
            PrototypeRendererData prototypeRendererData = GetPrototypeRenderData(id);
            if (prototypeRendererData != null)
            {
                prototypeRendererData.ClearPersistentData();
            }
        }

        public void ClearInstances()
        {
            foreach (PrototypeRendererData prototypeRenderData in PrototypeRenderDataList)
            {
                prototypeRenderData.ClearPersistentData();
            }
        }

        public PrototypeRendererData GetPrototypeRenderData(int id)
        {
            foreach (PrototypeRendererData item in PrototypeRenderDataList)
            {
                if (item.PrototypeID == id)
                {
                    return item;
                }
            }

            return null;
        }

        public void OnDisable()
        {
            foreach (PrototypeRendererData item in PrototypeRenderDataList)
            {
                item?.OnDisable();
            }
        }
    }
}
