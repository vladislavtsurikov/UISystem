#if UNITY_EDITOR
using VladislavTsurikov.MegaWorld.Editor.Common.Settings.FilterSettings.MaskFilterSystem;
using VladislavTsurikov.MegaWorld.Runtime.Common.Area;

namespace VladislavTsurikov.MegaWorld.Runtime.Common.Settings.FilterSettings.MaskFilterSystem.Utility
{
    public class MaskFilterVisualisation
    {
        protected MaskFilterContext _filterContext;

        public bool NeedUpdateMask = true;
        protected MaskFilterStack PastMaskFilterStack;

        protected virtual bool IsNeedUpdateMask(MaskFilterStack maskFilterStack, BoxArea boxArea)
        {
            NeedUpdateMask = true;
            return NeedUpdateMask;
        }

        public void DrawMaskFilterVisualization(MaskFilterStack maskFilterStack, BoxArea area, float multiplyAlpha = 1)
        {
            if (area.TerrainUnder == null)
            {
                return;
            }

            if (maskFilterStack.ElementList.Count > 0)
            {
                if (IsNeedUpdateMask(maskFilterStack, area))
                {
                    UpdateMask(maskFilterStack, area);
                }

                DrawShaderVisualisationUtility.DrawMaskFilter(area, _filterContext, multiplyAlpha);
            }
            else
            {
                DrawShaderVisualisationUtility.DrawAreaPreview(area);
            }
        }

        private void UpdateMask(MaskFilterStack maskFilterStack, BoxArea boxArea)
        {
            if (_filterContext == null)
            {
                _filterContext = new MaskFilterContext(boxArea);
            }
            else
            {
                _filterContext.Dispose();
            }

            FilterMaskOperation.UpdateFilterContext(ref _filterContext, maskFilterStack, boxArea);

            PastMaskFilterStack = maskFilterStack;

            NeedUpdateMask = false;
        }
    }
}
#endif
