using System;
using UnityEngine;
using VladislavTsurikov.MegaWorld.Editor.Common.Settings.FilterSettings;
using VladislavTsurikov.ReflectionUtility;

namespace VladislavTsurikov.MegaWorld.Runtime.Common.Settings.FilterSettings.MaskFilterSystem
{
    [Serializable]
    [Name("Slope")]
    public class SlopeFilter : MaskFilter
    {
        public BlendMode BlendMode = BlendMode.Multiply;

        public float MinSlope;
        public float MaxSlope = 20f;

        public FalloffType SlopeFalloffType = FalloffType.Add;
        public bool SlopeFalloffMinMax;

        public float MinAddSlopeFalloff = 30;
        public float MaxAddSlopeFalloff = 30;

        [Min(0)]
        public float AddSlopeFalloff = 30;

        private Material _slopeMat;

        private Material GetMaterial()
        {
            if (_slopeMat == null)
            {
                _slopeMat = new Material(Shader.Find("Hidden/MegaWorld/Slope"));
            }

            return _slopeMat;
        }

        public override void Eval(MaskFilterContext maskFilterContext, int index)
        {
            Material mat = GetMaterial();

            mat.SetTexture("_BaseMaskTex", maskFilterContext.SourceRenderTexture);
            mat.SetTexture("_NormalTex", maskFilterContext.NormalContext.sourceRenderTexture);

            SetMaterial(mat, index);

            Graphics.Blit(maskFilterContext.SourceRenderTexture, maskFilterContext.DestinationRenderTexture, mat, 0);
        }

        public void SetMaterial(Material mat, int index)
        {
            if (index == 0)
            {
                mat.SetInt("_BlendMode", (int)BlendMode.Multiply);
            }
            else
            {
                mat.SetInt("_BlendMode", (int)BlendMode);
            }

            mat.SetFloat("_MinSlope", MinSlope);
            mat.SetFloat("_MaxSlope", MaxSlope);

            switch (SlopeFalloffType)
            {
                case FalloffType.Add:
                {
                    var localMinAddSlopeFalloff = AddSlopeFalloff;
                    var localMaxAddSlopeFalloff = AddSlopeFalloff;

                    if (SlopeFalloffMinMax)
                    {
                        localMinAddSlopeFalloff = MinAddSlopeFalloff;
                        localMaxAddSlopeFalloff = MaxAddSlopeFalloff;
                    }

                    mat.SetInt("_SlopeFalloffType", 1);
                    mat.SetFloat("_MinAddSlopeFalloff", localMinAddSlopeFalloff);
                    mat.SetFloat("_MaxAddSlopeFalloff", localMaxAddSlopeFalloff);

                    break;
                }
                default:
                {
                    mat.SetInt("_SlopeFalloffType", 0);
                    break;
                }
            }
        }
    }
}
