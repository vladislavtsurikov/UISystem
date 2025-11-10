using System;
using UnityEngine;
using VladislavTsurikov.ReflectionUtility;
using VladislavTsurikov.UnityUtility.Runtime;

namespace VladislavTsurikov.MegaWorld.Runtime.Common.Settings.FilterSettings.MaskFilterSystem
{
    [Serializable]
    [Name("Concavity")]
    public class ConcavityFilter : MaskFilter
    {
        public enum ConcavityMode
        {
            Recessed = 0,
            Exposed = 1
        }

        public float ConcavityEpsilon = 1.0f; //kernel size

        public float
            ConcavityScalar = 1.0f; //toggles the compute shader between recessed (1.0f) & exposed (-1.0f) surfaces

        public float ConcavityStrength = 1.0f; //overall strength of the effect
        public AnimationCurve ConcavityRemapCurve = new(new Keyframe(0, 0), new Keyframe(1, 1));
        public Texture2D ConcavityRemapTex;

        private readonly int _remapTexWidth = 1024;

        //Compute Shader resource helper
        private ComputeShader _concavityCs;

        private Texture2D GetRemapTexture()
        {
            if (ConcavityRemapTex == null)
            {
                ConcavityRemapTex = new Texture2D(_remapTexWidth, 1, TextureFormat.RFloat, false, true)
                {
                    wrapMode = TextureWrapMode.Clamp
                };

                TextureUtility.AnimationCurveToTexture(ConcavityRemapCurve, ref ConcavityRemapTex);
            }

            return ConcavityRemapTex;
        }

        private ComputeShader GetComputeShader()
        {
            if (_concavityCs == null)
            {
                _concavityCs = (ComputeShader)Resources.Load("Concavity");
            }

            return _concavityCs;
        }

        public override void Eval(MaskFilterContext maskFilterContext, int index)
        {
            ComputeShader cs = GetComputeShader();
            var kidx = cs.FindKernel("ConcavityMultiply");

            Texture2D remapTex = GetRemapTexture();

            cs.SetTexture(kidx, "In_BaseMaskTex", maskFilterContext.SourceRenderTexture);
            cs.SetTexture(kidx, "In_HeightTex", maskFilterContext.HeightContext.sourceRenderTexture);
            cs.SetTexture(kidx, "OutputTex", maskFilterContext.DestinationRenderTexture);
            cs.SetTexture(kidx, "RemapTex", remapTex);
            cs.SetInt("RemapTexRes", remapTex.width);
            cs.SetFloat("EffectStrength", ConcavityStrength);
            cs.SetVector("TextureResolution",
                new Vector4(maskFilterContext.SourceRenderTexture.width, maskFilterContext.SourceRenderTexture.height,
                    ConcavityEpsilon, ConcavityScalar));

            //using 1s here so we don't need a multiple-of-8 texture in the compute shader (probably not optimal?)
            cs.Dispatch(kidx, maskFilterContext.SourceRenderTexture.width, maskFilterContext.SourceRenderTexture.height,
                1);
        }
    }
}
