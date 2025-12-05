#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using VladislavTsurikov.IMGUIUtility.Editor;
using VladislavTsurikov.MegaWorld.Runtime.Core;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas;

namespace VladislavTsurikov.MegaWorld.Editor.Core
{
    public static class ToolEditorUtility
    {
        public static bool DrawWarningAboutUnsupportedResourceType(SelectionData selectionData, Type toolType)
        {
            if (selectionData.SelectedData.HasOneSelectedGroup())
            {
                if (!ToolUtility.IsToolSupportSelectedResourcesType(toolType, selectionData))
                {
                    List<Type> supportedResourceTypes = ToolUtility.GetSupportedPrototypeTypes(toolType);

                    var text = "";

                    for (var i = 0; i < supportedResourceTypes.Count; i++)
                    {
                        var name = supportedResourceTypes[i].Name.Split('/').Last();

                        if (i == supportedResourceTypes.Count - 1)
                        {
                            text += name;
                        }
                        else
                        {
                            text += name + ", ";
                        }
                    }

                    CustomEditorGUILayout.HelpBox("This tool only works with these Resource Types: " + text);

                    return false;
                }

                return true;
            }

            if (!ToolUtility.IsToolSupportSelectedMultipleTypes(toolType, selectionData))
            {
                CustomEditorGUILayout.HelpBox("This tool does not support multiple selected types.");
                return false;
            }

            return true;
        }
    }
}
#endif
