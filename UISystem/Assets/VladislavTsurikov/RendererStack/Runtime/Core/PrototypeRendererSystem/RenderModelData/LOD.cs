using System.Collections.Generic;
using UnityEngine;

namespace VladislavTsurikov.RendererStack.Runtime.Core.PrototypeRendererSystem.RenderModelData
{
    public class LOD
    {
        public float Distance = 0;
        public MaterialPropertyBlock MaterialPropertyBlock;
        public List<Material> Materials = new();
        public Mesh Mesh;
        public MaterialPropertyBlock ShadowMaterialPropertyBlock;
    }
}
