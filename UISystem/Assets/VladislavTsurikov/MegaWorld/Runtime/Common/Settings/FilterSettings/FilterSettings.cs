using OdinSerializer;
using VladislavTsurikov.ComponentStack.Runtime.Core;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.FilterSettings.MaskFilterSystem;
using VladislavTsurikov.ReflectionUtility;

namespace VladislavTsurikov.MegaWorld.Runtime.Common.Settings.FilterSettings
{
    public enum FilterType
    {
        SimpleFilter,
        MaskFilter
    }

    [Name("Filter Settings")]
    public class FilterSettings : Component
    {
        public FilterType FilterType = FilterType.MaskFilter;

        [OdinSerialize]
        public MaskFilterComponentSettings MaskFilterComponentSettings = new();

        [OdinSerialize]
        public SimpleFilter SimpleFilter = new();
    }
}
