using System;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group;

namespace VladislavTsurikov.MegaWorld.Runtime.Common.Settings.FilterSettings.Utility
{
    public class SimpleFilterUtility
    {
        public static bool HasOneActiveSimpleFilter(Type toolType, SelectedData selectedVariables)
        {
            foreach (Group group in selectedVariables.SelectedGroupList)
            {
                var filterComponent = (FilterSettings)group.GetElement(toolType, typeof(FilterSettings));

                if (filterComponent == null)
                {
                    continue;
                }

                if (filterComponent.FilterType == FilterType.SimpleFilter)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
