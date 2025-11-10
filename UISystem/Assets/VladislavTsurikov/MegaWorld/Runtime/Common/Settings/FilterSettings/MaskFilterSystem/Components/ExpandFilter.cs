using UnityEngine;
using VladislavTsurikov.ReflectionUtility;

namespace VladislavTsurikov.MegaWorld.Runtime.Common.Settings.FilterSettings.MaskFilterSystem
{
    [Name("Expand")]
    public class ExpandFilter : MaskFilter
    {
        private const float MaxKernelSizeForNextIteration = 1;
        private static Material _material;

        public float KernelSize = 1f;

        public static Material Material
        {
            get
            {
                if (_material == null)
                {
                    _material = new Material(Shader.Find("Hidden/MegaWorld/Expand"));
                }

                return _material;
            }
        }

        public override void Eval(MaskFilterContext maskFilterContext, int index)
        {
            var amountKernelSize = KernelSize;

            while (amountKernelSize != 0)
            {
                float localKernelSize;

                if (amountKernelSize >= MaxKernelSizeForNextIteration)
                {
                    amountKernelSize -= MaxKernelSizeForNextIteration;

                    localKernelSize = MaxKernelSizeForNextIteration;
                }
                else
                {
                    localKernelSize = amountKernelSize;
                    amountKernelSize = 0;
                }

                Material.SetTexture("_MainTex", maskFilterContext.SourceRenderTexture);
                Material.SetFloat("KernelSize", localKernelSize);
                Material.SetFloat("MaxKernelSize", MaxKernelSizeForNextIteration);

                var temporaryTextureSource =
                    RenderTexture.GetTemporary(maskFilterContext.DestinationRenderTexture.descriptor);

                Graphics.Blit(maskFilterContext.SourceRenderTexture, temporaryTextureSource, Material, 0);
                Graphics.Blit(temporaryTextureSource, maskFilterContext.DestinationRenderTexture, Material, 1);

                Graphics.Blit(maskFilterContext.DestinationRenderTexture, maskFilterContext.SourceRenderTexture);

                RenderTexture.ReleaseTemporary(temporaryTextureSource);
            }
        }
    }
}
