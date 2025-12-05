using System;
using System.Collections.Generic;
using System.Linq;
using VladislavTsurikov.AttributeUtility.Runtime;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Attributes;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes;

namespace VladislavTsurikov.MegaWorld.Runtime.Core
{
    public static class ToolUtility
    {
        public static List<Type> GetSupportedPrototypeTypes(Type toolType) =>
            toolType.GetAttribute<SupportedPrototypeTypesAttribute>().PrototypeTypes.ToList();

        public static bool IsToolSupportSelectedResourcesType(Type toolType, SelectionData selectionData)
        {
            if (selectionData.SelectedData.HasOneSelectedGroup())
            {
                if (GetSupportedPrototypeTypes(toolType)
                    .Contains(selectionData.SelectedData.SelectedGroup.PrototypeType))
                {
                    return true;
                }
            }

            return false;
        }

        public static bool IsToolSupportSelectedMultipleTypes(Type toolType, SelectionData selectionData)
        {
            if (selectionData.SelectedData.SelectedGroupList.Count > 1)
            {
                if (toolType.GetAttribute<SupportMultipleSelectedGroupsAttribute>() != null)
                {
                    return true;
                }
            }

            return false;
        }

        public static bool IsToolSupportSelectedData(Type toolType, SelectionData selectionData)
        {
            if (selectionData.SelectedData.HasOneSelectedGroup())
            {
                if (GetSupportedPrototypeTypes(toolType)
                    .Contains(selectionData.SelectedData.SelectedGroup.PrototypeType))
                {
                    return true;
                }
            }
            else
            {
                if (toolType.GetAttribute<SupportMultipleSelectedGroupsAttribute>() != null)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
