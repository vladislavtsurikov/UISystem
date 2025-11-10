#if UNITY_EDITOR
using UnityEditor.TerrainTools;
using UnityEngine;
using UnityEngine.TerrainTools;
using VladislavTsurikov.MegaWorld.Runtime.Common.Area;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.FilterSettings.MaskFilterSystem;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.FilterSettings.MaskFilterSystem.Utility;
using VladislavTsurikov.MegaWorld.Runtime.Core.PreferencesSystem;
using VladislavTsurikov.UnityUtility.Runtime;

namespace VladislavTsurikov.MegaWorld.Editor.Common.Settings.FilterSettings.MaskFilterSystem
{
    public static class DrawShaderVisualisationUtility
    {
        private static readonly int _alphaVisualisationType = Shader.PropertyToID("_AlphaVisualisationType");
        private static readonly int _alpha = Shader.PropertyToID("_Alpha");
        private static readonly int _colorSpace = Shader.PropertyToID("_ColorSpace");
        private static readonly int _enableBrushStripe = Shader.PropertyToID("_EnableBrushStripe");
        private static readonly int _color = Shader.PropertyToID("_Color");
        private static readonly int _blendParams = Shader.PropertyToID("_BlendParams");
        private static readonly int _blendTex = Shader.PropertyToID("_BlendTex");

        public static void DrawAreaPreview(BoxArea boxArea)
        {
            if (boxArea.TerrainUnder == null)
            {
                return;
            }

            var terrainPainterRenderHelper = new TerrainPainterRenderHelper(boxArea);

            PaintContext heightContext = terrainPainterRenderHelper.AcquireHeightmap();

            if (heightContext == null)
            {
                return;
            }

#if UNITY_2021_2_OR_NEWER
            terrainPainterRenderHelper.RenderAreaPreview(heightContext, TerrainBrushPreviewMode.SourceRenderTexture,
                TerrainPaintUtilityEditor.GetDefaultBrushPreviewMaterial(), 0);
#else
            terrainPainterRenderHelper.RenderAreaPreview(heightContext, TerrainPaintUtilityEditor.BrushPreview.SourceRenderTexture, TerrainPaintUtilityEditor.GetDefaultBrushPreviewMaterial(), 0);
#endif

            TerrainPaintUtility.ReleaseContextResources(heightContext);
        }

        public static void DrawMaskFilter(BoxArea boxArea, MaskFilterContext filterContext, float multiplyAlpha = 1)
        {
            if (boxArea.TerrainUnder == null)
            {
                return;
            }

            var terrainPainterRenderHelper = new TerrainPainterRenderHelper(boxArea);

            Texture brushTexture = terrainPainterRenderHelper.BoxArea.Mask;

            Material brushMaterial = MaskFilterUtility.GetBrushPreviewMaterial();
            RenderTexture tmpRT = RenderTexture.active;

            RenderTexture filterMaskRT = filterContext.Output;

            if (filterMaskRT != null)
            {
                VisualisationMaskFiltersPreference visualisationMaskFiltersPreference =
                    PreferenceElementSingleton<VisualisationMaskFiltersPreference>.Instance;

                //Composite the brush texture onto the filter stack result
                var compRT = RenderTexture.GetTemporary(filterMaskRT.descriptor);
                Material blendMat = MaskFilterUtility.GetBlendMaterial();
                blendMat.SetTexture(_blendTex, brushTexture);
                blendMat.SetVector(_blendParams,
                    new Vector4(0.0f, 0.0f, -(terrainPainterRenderHelper.BoxArea.Rotation * Mathf.Deg2Rad), 0.0f));
                TerrainPaintUtility.SetupTerrainToolMaterialProperties(filterContext.HeightContext,
                    terrainPainterRenderHelper.BrushTransform, blendMat);
                Graphics.Blit(filterMaskRT, compRT, blendMat, 0);

                RenderTexture.active = tmpRT;

                BrushTransform identityBrushTransform = TerrainPaintUtility.CalculateBrushTransform(
                    terrainPainterRenderHelper.BoxArea.TerrainUnder,
                    UnityTerrainUtility.GetTextureCoordFromUnityTerrain(terrainPainterRenderHelper.BoxArea.Center),
                    terrainPainterRenderHelper.BoxArea.BoxSize, 0.0f);

                brushMaterial.SetColor(_color, visualisationMaskFiltersPreference.Color);
                brushMaterial.SetInt(_enableBrushStripe, visualisationMaskFiltersPreference.EnableStripe ? 1 : 0);
                brushMaterial.SetInt(_colorSpace, (int)visualisationMaskFiltersPreference.ColorSpace);
                brushMaterial.SetInt(_alphaVisualisationType,
                    (int)visualisationMaskFiltersPreference.AlphaVisualisationType);
                brushMaterial.SetFloat(_alpha, visualisationMaskFiltersPreference.CustomAlpha * multiplyAlpha);

                TerrainPaintUtility.SetupTerrainToolMaterialProperties(filterContext.HeightContext,
                    identityBrushTransform, brushMaterial);
#if UNITY_2021_2_OR_NEWER
                TerrainPaintUtilityEditor.DrawBrushPreview(filterContext.HeightContext,
                    TerrainBrushPreviewMode.SourceRenderTexture, compRT, identityBrushTransform, brushMaterial, 0);
#else
    			TerrainPaintUtilityEditor.DrawBrushPreview(heightContext, TerrainPaintUtilityEditor.BrushPreview.SourceRenderTexture, compRT, identityBrushTransform, brushMaterial, 0);
#endif
                RenderTexture.ReleaseTemporary(compRT);
            }
        }
    }
}
#endif
