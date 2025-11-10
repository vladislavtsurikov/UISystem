using System;
using UnityEngine;

namespace VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.Data.RendererData
{
    [Serializable]
    public struct Instance
    {
        public int ID;
        public Vector3 Position;
        public Vector3 Scale;
        public Quaternion Rotation;

        public static Instance ConvertToSerializableInstance(TerrainObjectInstance instance)
        {
            Instance item;
            item.ID = instance.ID;
            item.Position = instance.Position;
            item.Rotation = instance.Rotation;
            item.Scale = instance.Scale;

            return item;
        }
    }
}
