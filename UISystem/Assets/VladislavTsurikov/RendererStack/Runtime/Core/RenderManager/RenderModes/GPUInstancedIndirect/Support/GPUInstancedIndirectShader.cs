using System;
using UnityEngine;

namespace VladislavTsurikov.RendererStack.Runtime.Core.RenderManager.GPUInstancedIndirect
{
    [Serializable]
    public class GPUInstancedIndirectShader
    {
        public Shader Shader;
        public string OriginalName;
        public bool IsOriginal;

        public GPUInstancedIndirectShader(string originalName, Shader shader, bool isOriginal)
        {
            OriginalName = originalName;
            Shader = shader;
            IsOriginal = isOriginal;
        }
    }
}
