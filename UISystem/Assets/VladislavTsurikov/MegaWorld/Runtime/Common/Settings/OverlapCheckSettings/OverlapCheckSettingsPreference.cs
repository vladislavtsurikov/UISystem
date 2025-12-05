using VladislavTsurikov.MegaWorld.Runtime.Core.PreferencesSystem;
using VladislavTsurikov.ReflectionUtility;

namespace VladislavTsurikov.MegaWorld.Runtime.Common.Settings.OverlapCheckSettings
{
    [Name("Overlap Check Settings")]
    public class OverlapCheckSettingsPreference : PreferenceSettings
    {
        public float MultiplyFindSize = 1f;
    }
}
