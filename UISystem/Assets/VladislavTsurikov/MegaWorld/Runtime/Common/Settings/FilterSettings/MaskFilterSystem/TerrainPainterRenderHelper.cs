using UnityEngine;
using UnityEngine.TerrainTools;
using VladislavTsurikov.MegaWorld.Runtime.Common.Area;
using VladislavTsurikov.UnityUtility.Runtime;
#if UNITY_EDITOR
using UnityEditor.TerrainTools;
#endif

namespace VladislavTsurikov.MegaWorld.Runtime.Common.Settings.FilterSettings.MaskFilterSystem
{
    public class TerrainPainterRenderHelper
    {
        private readonly BrushTransform _brushTransform;
        private readonly Terrain _terrainUnderCursor;
        private readonly Rect _brushRect;

        public BoxArea BoxArea { get; }
        public BrushTransform BrushTransform => _brushTransform;

        public TerrainPainterRenderHelper(BoxArea boxArea)
        {
            BoxArea = boxArea;

            _terrainUnderCursor = UnityTerrainUtility.GetTerrain(boxArea.Center);

            Vector2 currUV = UnityTerrainUtility.WorldPointToUV(boxArea.Center, _terrainUnderCursor);

            _brushTransform =
                TerrainPaintUtility.CalculateBrushTransform(_terrainUnderCursor, currUV, boxArea.BoxSize,
                    boxArea.Rotation);
            _brushRect = _brushTransform.GetBrushXYBounds();
        }

        #region Rendering

        public void RenderBrush(PaintContext paintContext, Material material, int pass)
        {
            Texture sourceTexture = paintContext.sourceRenderTexture;
            RenderTexture destinationTexture = paintContext.destinationRenderTexture;

            Graphics.Blit(sourceTexture, destinationTexture, material, pass);
        }

#if UNITY_EDITOR
#if UNITY_2021_2_OR_NEWER
        public void RenderAreaPreview(PaintContext paintContext, TerrainBrushPreviewMode previewTexture,
            Material material, int pass) =>
            TerrainPaintUtilityEditor.DrawBrushPreview(paintContext, previewTexture, BoxArea.Mask, _brushTransform,
                material, pass);
#else
		public void RenderAreaPreview(PaintContext paintContext, TerrainPaintUtilityEditor.BrushPreview previewTexture, Material material, int pass)
		{
			TerrainPaintUtilityEditor.DrawBrushPreview(paintContext, previewTexture, _areaVariables.Mask, _brushTransform, material, pass);
		}
#endif
#endif

        #endregion

        #region Material Set-up

        public void SetupTerrainToolMaterialProperties(PaintContext paintContext, Material material) =>
            TerrainPaintUtility.SetupTerrainToolMaterialProperties(paintContext, _brushTransform, material);

        #endregion

        #region Texture Acquisition

        public PaintContext AcquireHeightmap(int extraBorderPixels = 0) =>
            TerrainPaintUtility.BeginPaintHeightmap(_terrainUnderCursor, _brushRect, extraBorderPixels);

        public PaintContext AcquireTexture(TerrainLayer layer, int extraBorderPixels = 0) =>
            TerrainPaintUtility.BeginPaintTexture(_terrainUnderCursor, _brushRect, layer, extraBorderPixels);

        public PaintContext AcquireNormalmap(int extraBorderPixels = 0) =>
            TerrainPaintUtility.CollectNormals(_terrainUnderCursor, _brushRect, extraBorderPixels);

        public PaintContext AcquireHolesTexture(int extraBorderPixels = 0)
        {
#if UNITY_2019_3_OR_NEWER
            return TerrainPaintUtility.BeginPaintHoles(_terrainUnderCursor, _brushRect, extraBorderPixels);
#else
			return null;
#endif
        }

        #endregion
    }
}
