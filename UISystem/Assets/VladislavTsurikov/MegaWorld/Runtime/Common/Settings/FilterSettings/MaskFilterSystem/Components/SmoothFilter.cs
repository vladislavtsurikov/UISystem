using UnityEngine;
using VladislavTsurikov.ReflectionUtility;

namespace VladislavTsurikov.MegaWorld.Runtime.Common.Settings.FilterSettings.MaskFilterSystem
{
    [Name("Smooth")]
    public class SmoothFilter : MaskFilter
    {
        private static Material _material;
        public float SmoothBlurRadius = 1f;
        public float SmoothVerticality;

        public static Material Material
        {
            get
            {
                if (_material == null)
                {
                    _material = new Material(Shader.Find("Hidden/MegaWorld/SmoothHeight"));
                }

                return _material;
            }
        }

        public override void Eval(MaskFilterContext maskFilterContext, int index)
        {
            Material filterMat = Material;

            var brushParams = new Vector4(Mathf.Clamp(1, 0.0f, 1.0f), 0.0f, 0.0f, 0.0f);
            filterMat.SetTexture("_MainTex", maskFilterContext.SourceRenderTexture);
            filterMat.SetTexture("_BrushTex", Texture2D.whiteTexture);
            filterMat.SetVector("_BrushParams", brushParams);
            var smoothWeights = new Vector4(
                Mathf.Clamp01(1.0f - Mathf.Abs(SmoothVerticality)), // centered
                Mathf.Clamp01(-SmoothVerticality), // min
                Mathf.Clamp01(SmoothVerticality), // max
                SmoothBlurRadius); // kernel size
            filterMat.SetVector("_SmoothWeights", smoothWeights);

            var tmpRT = RenderTexture.GetTemporary(maskFilterContext.DestinationRenderTexture.descriptor);
            Graphics.Blit(maskFilterContext.SourceRenderTexture, tmpRT, filterMat, 0);
            Graphics.Blit(tmpRT, maskFilterContext.DestinationRenderTexture, filterMat, 1);

            RenderTexture.ReleaseTemporary(tmpRT);
        }
    }
}
