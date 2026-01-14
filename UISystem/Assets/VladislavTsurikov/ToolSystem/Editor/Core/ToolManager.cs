using System;
using System.Collections.Generic;
using System.Linq;
using VladislavTsurikov.AttributeUtility.Runtime;
using VladislavTsurikov.ReflectionUtility.Runtime;
using VladislavTsurikov.ToolSystem.Runtime.Core;
using VladislavTsurikov.ToolSystem.Runtime.Core.Attributes;

namespace VladislavTsurikov.ToolSystem.Editor.Core
{
    public static class ToolManager
    {
        private static List<Type> _allToolTypes;
        private static Dictionary<string, List<Type>> _toolsByGroup;

        public static List<Type> GetAllToolTypes()
        {
            if (_allToolTypes == null)
            {
                DiscoverTools();
            }

            return _allToolTypes;
        }

        public static Dictionary<string, List<Type>> GetToolsByGroup()
        {
            if (_toolsByGroup == null)
            {
                DiscoverTools();
            }

            return _toolsByGroup;
        }

        public static string GetToolName(Type toolType)
        {
            var toolAttribute = toolType.GetAttribute<ToolAttribute>();
            if (toolAttribute != null)
            {
                return toolAttribute.Name;
            }

            var nameAttribute = toolType.GetAttribute<NameAttribute>();
            if (nameAttribute != null)
            {
                return nameAttribute.Name;
            }

            return toolType.Name;
        }

        public static string GetToolDescription(Type toolType)
        {
            var toolAttribute = toolType.GetAttribute<ToolAttribute>();
            return toolAttribute?.Description ?? string.Empty;
        }

        public static string GetToolGroup(Type toolType)
        {
            var groupAttribute = toolType.GetAttribute<ToolGroupAttribute>();
            return groupAttribute?.GroupName ?? "Default";
        }

        public static string GetToolDocumentation(Type toolType)
        {
            var docAttribute = toolType.GetAttribute<ToolDocumentationAttribute>();
            return docAttribute?.Url ?? string.Empty;
        }

        private static void DiscoverTools()
        {
            _allToolTypes = AllTypesDerivedFrom<EditorTool>.Types.ToList();
            _toolsByGroup = new Dictionary<string, List<Type>>();

            foreach (Type toolType in _allToolTypes)
            {
                if (toolType.IsAbstract)
                    continue;

                string groupName = GetToolGroup(toolType);

                if (!_toolsByGroup.ContainsKey(groupName))
                {
                    _toolsByGroup[groupName] = new List<Type>();
                }

                _toolsByGroup[groupName].Add(toolType);
            }
        }
    }
}
