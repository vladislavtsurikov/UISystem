using UnityEngine;
using UnityEngine.TerrainTools;
using VladislavTsurikov.MegaWorld.Runtime.Common.Area;
using VladislavTsurikov.UnityUtility.Runtime;
#if !UNITY_2021_2_OR_NEWER
using UnityEngine.Experimental.TerrainAPI;
#endif

namespace VladislavTsurikov.MegaWorld.Runtime.Common.Settings.FilterSettings.MaskFilterSystem.Utility
{
    public static class FilterMaskOperation
    {
        public static Texture2D UpdateMaskTexture(MaskFilterComponentSettings maskFilterComponentSettings,
            BoxArea boxArea)
        {
            if (boxArea.TerrainUnder == null)
            {
                return null;
            }

            if (maskFilterComponentSettings.MaskFilterStack.ElementList.Count != 0)
            {
                UpdateFilterContext(ref maskFilterComponentSettings.FilterContext,
                    maskFilterComponentSettings.MaskFilterStack, boxArea);
                RenderTexture filterMaskRT = maskFilterComponentSettings.FilterContext.Output;
                maskFilterComponentSettings.FilterMaskTexture2D = TextureUtility.ToTexture2D(filterMaskRT);
                DisposeMaskTexture(maskFilterComponentSettings);

                return maskFilterComponentSettings.FilterMaskTexture2D;
            }

            return null;
        }

        public static void UpdateFilterContext(ref MaskFilterContext filterContext, MaskFilterStack maskFilterStack,
            BoxArea boxArea)
        {
            if (filterContext != null)
            {
                filterContext.Dispose();
            }

            var terrainPainterRenderHelper = new TerrainPainterRenderHelper(boxArea);

            PaintContext heightContext = terrainPainterRenderHelper.AcquireHeightmap();
            PaintContext normalContext = terrainPainterRenderHelper.AcquireNormalmap();

            var output = new RenderTexture(heightContext.sourceRenderTexture.width,
                heightContext.sourceRenderTexture.height, heightContext.sourceRenderTexture.depth,
                RenderTextureFormat.ARGB32) { enableRandomWrite = true };
            output.Create();

            filterContext = new MaskFilterContext(maskFilterStack, heightContext, normalContext, output,
                terrainPainterRenderHelper.BoxArea);
        }

        private static void DisposeMaskTexture(MaskFilterComponentSettings maskFilterComponentSettings)
        {
            if (maskFilterComponentSettings.FilterContext != null)
            {
                maskFilterComponentSettings.FilterContext.Dispose();
                maskFilterComponentSettings.FilterContext = null;
            }
        }
    }
}
