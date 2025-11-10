using System;
using UnityEngine;

namespace VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.PrototypeTerrainTexture
{
    [Serializable]
    public sealed class TerrainTexture
    {
        public Texture2D Texture;
        public int TerrainProtoId;
        public bool Selected;

        public void CopyFrom(TerrainTexture other)
        {
            Texture = other.Texture;
            TerrainProtoId = other.TerrainProtoId;
            Selected = other.Selected;
        }
    }
}
