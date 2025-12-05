using System;
using UnityEngine;
using VladislavTsurikov.ReflectionUtility;
using VladislavTsurikov.UnityUtility.Runtime;

namespace VladislavTsurikov.MegaWorld.Runtime.Common.Settings.FilterSettings.MaskFilterSystem
{
    [Serializable]
    [Name("Aspect")]
    public class AspectFilter : MaskFilter
    {
        private static readonly int _sRemapTexWidth = 1024;
        public BlendMode BlendMode = BlendMode.Multiply;
        public float Rotation;
        public float Epsilon = 1.0f; //kernel size
        public float EffectStrength = 1.0f; //overall strength of the effect
        public AnimationCurve RemapCurve = new(new Keyframe(0, 0), new Keyframe(1, 1));
        public Texture2D RemapTex;

        //Compute Shader resource helper
        private ComputeShader _mAspectCs;

        private Texture2D GetRemapTexture()
        {
            if (RemapTex == null)
            {
                RemapTex = new Texture2D(_sRemapTexWidth, 1, TextureFormat.RFloat, false, true)
                {
                    wrapMode = TextureWrapMode.Clamp
                };

                TextureUtility.AnimationCurveToTexture(RemapCurve, ref RemapTex);
            }

            return RemapTex;
        }

        private ComputeShader GetComputeShader()
        {
            if (_mAspectCs == null)
            {
                _mAspectCs = (ComputeShader)Resources.Load("Aspect");
            }

            return _mAspectCs;
        }

        public override void Eval(MaskFilterContext maskFilterContext, int index)
        {
            ComputeShader cs = GetComputeShader();
            var kidx = cs.FindKernel("AspectRemap");

            //using 1s here so we don't need a multiple-of-8 texture in the compute shader (probably not optimal?)
            int[] numWorkGroups = { 1, 1, 1 };

            Texture2D remapTex = GetRemapTexture();

            //float rotRad = (fc.properties["brushRotation"] - 90.0f) * Mathf.Deg2Rad;
            var rotRad = (Rotation - 90.0f) * Mathf.Deg2Rad;

            if (index == 0)
            {
                cs.SetInt("_BlendMode", (int)BlendMode.Multiply);
            }
            else
            {
                cs.SetInt("_BlendMode", (int)BlendMode);
            }

            cs.SetTexture(kidx, "In_BaseMaskTex", maskFilterContext.SourceRenderTexture);
            cs.SetTexture(kidx, "In_HeightTex", maskFilterContext.HeightContext.sourceRenderTexture);
            cs.SetTexture(kidx, "OutputTex", maskFilterContext.DestinationRenderTexture);
            cs.SetTexture(kidx, "RemapTex", remapTex);
            cs.SetInt("RemapTexRes", remapTex.width);
            cs.SetFloat("EffectStrength", EffectStrength);
            cs.SetVector("TextureResolution",
                new Vector4(maskFilterContext.SourceRenderTexture.width, maskFilterContext.SourceRenderTexture.height,
                    0.0f, 0.0f));
            cs.SetVector("AspectValues", new Vector4(Mathf.Cos(rotRad), Mathf.Sin(rotRad), Epsilon, 0.0f));

            cs.Dispatch(kidx, maskFilterContext.SourceRenderTexture.width / numWorkGroups[0],
                maskFilterContext.SourceRenderTexture.height / numWorkGroups[1], numWorkGroups[2]);
        }
    }
}
