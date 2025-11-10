using System.Collections.Generic;
using UnityEngine;

namespace VladislavTsurikov.RendererStack.Runtime.Core.PrototypeRendererSystem.RenderModelData
{
    public class RenderModel
    {
        public float BoundingSphereRadius;
        public List<LOD> LODs = new();
        public Vector3 MultiplySize;
        public GameObject Prefab;

        public RenderModel(GameObject prefab) => Prefab = prefab;

        public void SetLODFadeKeyword(bool useLodFade)
        {
            foreach (LOD lod in LODs)
            foreach (Material mat in lod.Materials)
            {
                if (useLodFade)
                {
                    mat.EnableKeyword("LOD_FADE_CROSSFADE");
                }
                else
                {
                    mat.DisableKeyword("LOD_FADE_CROSSFADE");
                }
            }
        }
    }
}
