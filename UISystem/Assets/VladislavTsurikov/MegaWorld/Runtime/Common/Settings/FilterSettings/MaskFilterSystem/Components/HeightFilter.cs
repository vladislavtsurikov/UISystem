using System;
using UnityEngine;
using VladislavTsurikov.MegaWorld.Editor.Common.Settings.FilterSettings;
using VladislavTsurikov.ReflectionUtility;

namespace VladislavTsurikov.MegaWorld.Runtime.Common.Settings.FilterSettings.MaskFilterSystem
{
    [Serializable]
    [Name("Height")]
    public class HeightFilter : MaskFilter
    {
        public BlendMode BlendMode = BlendMode.Multiply;

        public float MinHeight;
        public float MaxHeight;

        public FalloffType HeightFalloffType = FalloffType.Add;
        public bool HeightFalloffMinMax;

        public float MinAddHeightFalloff = 30;
        public float MaxAddHeightFalloff = 30;

        [Min(0)]
        public float AddHeightFalloff = 30;

        private ComputeShader _heightCs;

        public ComputeShader GetComputeShader()
        {
            if (_heightCs == null)
            {
                _heightCs = (ComputeShader)Resources.Load("Height");
            }

            return _heightCs;
        }

        public override void Eval(MaskFilterContext maskFilterContext, int index)
        {
            ComputeShader cs = GetComputeShader();
            var kidx = cs.FindKernel("Height");

            cs.SetTexture(kidx, "In_BaseMaskTex", maskFilterContext.SourceRenderTexture);
            cs.SetTexture(kidx, "In_HeightTex", maskFilterContext.HeightContext.sourceRenderTexture);
            cs.SetTexture(kidx, "OutputTex", maskFilterContext.DestinationRenderTexture);

            SetMaterial(cs, maskFilterContext, index);

            //using workgroup size of 1 here to avoid needing to resize render textures
            cs.Dispatch(kidx, maskFilterContext.SourceRenderTexture.width, maskFilterContext.SourceRenderTexture.height,
                1);
        }

        public void SetMaterial(ComputeShader cs, MaskFilterContext fс, int index)
        {
            Terrain terrain = fс.BoxArea.TerrainUnder;

            if (index == 0)
            {
                cs.SetInt("_BlendMode", (int)BlendMode.Multiply);
            }
            else
            {
                cs.SetInt("_BlendMode", (int)BlendMode);
            }

            cs.SetFloat("_MinHeight", MinHeight);
            cs.SetFloat("_MaxHeight", MaxHeight);

            Vector3 position = terrain.transform.position;
            cs.SetFloat("_ClampMinHeight", position.y);
            cs.SetFloat("_ClampMaxHeight", terrain.terrainData.size.y + position.y);

            switch (HeightFalloffType)
            {
                case FalloffType.Add:
                {
                    var localMinAddHeightFalloff = AddHeightFalloff;
                    var localMaxAddHeightFalloff = AddHeightFalloff;

                    if (HeightFalloffMinMax)
                    {
                        localMinAddHeightFalloff = MinAddHeightFalloff;
                        localMaxAddHeightFalloff = MaxAddHeightFalloff;
                    }

                    cs.SetInt("_HeightFalloffType", 1);
                    cs.SetFloat("_MinAddHeightFalloff", localMinAddHeightFalloff);
                    cs.SetFloat("_MaxAddHeightFalloff", localMaxAddHeightFalloff);
                    break;
                }
                default:
                {
                    cs.SetInt("_HeightFalloffType", 0);
                    break;
                }
            }
        }
    }
}
