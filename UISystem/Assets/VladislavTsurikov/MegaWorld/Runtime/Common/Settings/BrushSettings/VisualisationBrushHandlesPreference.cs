using UnityEngine;
using VladislavTsurikov.MegaWorld.Runtime.Core.PreferencesSystem;
using VladislavTsurikov.ReflectionUtility;

namespace VladislavTsurikov.MegaWorld.Runtime.Common.Settings.BrushSettings
{
    [Name("Visualisation Brush Handles")]
    public class VisualisationBrushHandlesPreference : PreferenceSettings
    {
        public Color CircleColor = new(0.2f, 0.5f, 0.7f, 1);
        public float CirclePixelWidth = 5f;
        public bool DrawSolidDisc = true;
    }
}
