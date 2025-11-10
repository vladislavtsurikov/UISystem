using UnityEngine;
using VladislavTsurikov.MegaWorld.Runtime.Core.PreferencesSystem;
using VladislavTsurikov.ReflectionUtility;

namespace VladislavTsurikov.MegaWorld.Runtime.Common.Settings.FilterSettings
{
    public enum ColorHandlesType
    {
        Custom,
        Standard
    }

    public enum HandleResizingType
    {
        Distance,
        Resolution,
        CustomSize
    }

    public enum HandlesType
    {
        Sphere,
        DotCap
    }

    [Name("Visualisation Simple Filter")]
    public class VisualisationSimpleFilterPreference : PreferenceSettings
    {
        public Color ActiveColor = Color.green;

        [Range(0f, 1f)]
        public float Alpha = 0.6f;

        public ColorHandlesType ColorHandlesType = ColorHandlesType.Standard;
        public float CustomHandleSize = 0.3f;
        public bool EnableSpawnVisualization = true;
        public HandleResizingType HandleResizingType = HandleResizingType.Distance;

        public HandlesType HandlesType = HandlesType.DotCap;
        public Color InactiveColor = Color.red;

        public int VisualiserResolution = 15;
    }
}
