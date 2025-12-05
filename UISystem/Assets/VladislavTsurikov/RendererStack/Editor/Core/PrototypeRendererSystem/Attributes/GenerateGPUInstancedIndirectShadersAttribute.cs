using UnityEngine;
using VladislavTsurikov.RendererStack.Runtime.Core.PrototypeRendererSystem;
using VladislavTsurikov.RendererStack.Runtime.Core.RenderManager.GPUInstancedIndirect;

namespace VladislavTsurikov.RendererStack.Editor.Core.PrototypeRendererSystem
{
    public class GenerateGPUInstancedIndirectShadersAttribute : ChangeShaderCodeAttribute
    {
        public override void ChangeShaderCode(Prototype proto)
        {
            if (Application.isPlaying)
            {
                return;
            }

            GPUInstancedIndirectShaderStack.Instance.ClearEmptyShaders();

#if UNITY_EDITOR
            GPUInstancedIndirectShaderUtility.GenerateInstancedShadersIfNecessary(proto);
#endif
        }
    }
}
