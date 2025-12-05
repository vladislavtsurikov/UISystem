using UnityEngine;
using VladislavTsurikov.MegaWorld.Runtime.Core.PreferencesSystem;
using VladislavTsurikov.ReflectionUtility;

namespace VladislavTsurikov.MegaWorld.Runtime.Common.Settings.FilterSettings.MaskFilterSystem
{
    public enum AlphaVisualisationType
    {
        Default,
        BrushFilter,
        None
    }

    public enum ColorSpaceForBrushMaskFilter
    {
        СustomColor,
        Colorful,
        Heightmap
    }

    [Name("Visualisation Mask Filters")]
    public class VisualisationMaskFiltersPreference : PreferenceSettings
    {
        public AlphaVisualisationType AlphaVisualisationType = AlphaVisualisationType.None;
        public Color Color = new(128, 171, 78, 255);
        public ColorSpaceForBrushMaskFilter ColorSpace = ColorSpaceForBrushMaskFilter.Colorful;
        public float CustomAlpha = 0.3f;
        public bool EnableStripe = true;
    }
}
