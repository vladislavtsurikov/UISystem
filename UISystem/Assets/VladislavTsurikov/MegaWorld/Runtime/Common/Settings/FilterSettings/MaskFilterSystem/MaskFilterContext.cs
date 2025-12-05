using UnityEngine;
using UnityEngine.TerrainTools;
using VladislavTsurikov.MegaWorld.Runtime.Common.Area;

namespace VladislavTsurikov.MegaWorld.Runtime.Common.Settings.FilterSettings.MaskFilterSystem
{
    public class MaskFilterContext
    {
        public readonly BoxArea BoxArea;
        public Vector3 BrushPos;
        public RenderTexture DestinationRenderTexture;
        public PaintContext HeightContext;
        public PaintContext NormalContext;
        public RenderTexture SourceRenderTexture;

        public MaskFilterContext(BoxArea boxArea) => BoxArea = boxArea;

        public MaskFilterContext(MaskFilterStack maskFilterStack, PaintContext heightContext,
            PaintContext normalContext, RenderTexture output, BoxArea boxArea)
        {
            BoxArea = boxArea;
            BrushPos = new Vector3(boxArea.Center.x, 0, boxArea.Center.z);
            SourceRenderTexture = null;
            DestinationRenderTexture = null;
            HeightContext = heightContext;
            NormalContext = normalContext;
            Output = output;
            DestinationRenderTexture = output;

            maskFilterStack.Eval(this);
        }

        public RenderTexture Output { get; private set; }

        public void Dispose()
        {
            if (HeightContext != null)
            {
                TerrainPaintUtility.ReleaseContextResources(HeightContext);
                HeightContext = null;
            }

            if (NormalContext != null)
            {
                TerrainPaintUtility.ReleaseContextResources(NormalContext);
                NormalContext = null;
            }

            if (Output != null)
            {
                Output.Release();
                Output = null;
            }
        }
    }
}
