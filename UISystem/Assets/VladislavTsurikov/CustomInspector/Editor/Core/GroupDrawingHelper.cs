#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using VladislavTsurikov.CustomInspector.Runtime;

namespace VladislavTsurikov.CustomInspector.Editor.Core
{
    /// <summary>
    /// Helper class for organizing fields into groups based on GroupAttribute
    /// </summary>
    public static class GroupDrawingHelper
    {
        public class GroupedFields<TFieldDrawer, TDecoratorDrawer>
            where TFieldDrawer : FieldDrawer
            where TDecoratorDrawer : DecoratorDrawer
        {
            public GroupAttribute GroupAttribute { get; set; }
            public string GroupPath { get; set; }
            public List<ProcessedFieldWrapper<TFieldDrawer, TDecoratorDrawer>> Fields { get; set; } = new();
        }

        public class ProcessedFieldWrapper<TFieldDrawer, TDecoratorDrawer>
            where TFieldDrawer : FieldDrawer
            where TDecoratorDrawer : DecoratorDrawer
        {
            public FieldInfo Field { get; set; }
            public string FieldName { get; set; }
            public TFieldDrawer Drawer { get; set; }
            public List<TDecoratorDrawer> Decorators { get; set; }
            public string Tooltip { get; set; }
            public object Value { get; set; }
        }

        /// <summary>
        /// Group fields by their GroupAttribute
        /// </summary>
        public static List<GroupedFields<TFieldDrawer, TDecoratorDrawer>> GroupFields<TFieldDrawer, TDecoratorDrawer>(
            IEnumerable<dynamic> processedFields)
            where TFieldDrawer : FieldDrawer
            where TDecoratorDrawer : DecoratorDrawer
        {
            var groups = new Dictionary<string, GroupedFields<TFieldDrawer, TDecoratorDrawer>>();
            var ungroupedFields = new List<ProcessedFieldWrapper<TFieldDrawer, TDecoratorDrawer>>();

            foreach (var processedField in processedFields)
            {
                var field = (FieldInfo)processedField.Field;
                var groupAttribute = field.GetCustomAttribute<GroupAttribute>();

                var wrapper = new ProcessedFieldWrapper<TFieldDrawer, TDecoratorDrawer>
                {
                    Field = field,
                    FieldName = processedField.FieldName,
                    Drawer = processedField.Drawer,
                    Decorators = processedField.Decorators,
                    Tooltip = processedField.Tooltip,
                    Value = processedField.Value
                };

                if (groupAttribute != null)
                {
                    string groupPath = groupAttribute.GroupPath;

                    if (!groups.ContainsKey(groupPath))
                    {
                        groups[groupPath] = new GroupedFields<TFieldDrawer, TDecoratorDrawer>
                        {
                            GroupAttribute = groupAttribute,
                            GroupPath = groupPath,
                            Fields = new List<ProcessedFieldWrapper<TFieldDrawer, TDecoratorDrawer>>()
                        };
                    }

                    groups[groupPath].Fields.Add(wrapper);
                }
                else
                {
                    ungroupedFields.Add(wrapper);
                }
            }

            // Create result list with ungrouped fields first, then groups
            var result = new List<GroupedFields<TFieldDrawer, TDecoratorDrawer>>();

            // Add ungrouped fields as a special group
            if (ungroupedFields.Count > 0)
            {
                result.Add(new GroupedFields<TFieldDrawer, TDecoratorDrawer>
                {
                    GroupAttribute = null,
                    GroupPath = null,
                    Fields = ungroupedFields
                });
            }

            // Add grouped fields, sorted by Order
            var sortedGroups = groups.Values.OrderBy(g => g.GroupAttribute.Order);
            result.AddRange(sortedGroups);

            return result;
        }

        /// <summary>
        /// Get the display name for a group (last segment of the path)
        /// </summary>
        public static string GetGroupDisplayName(string groupPath)
        {
            if (string.IsNullOrWhiteSpace(groupPath))
            {
                return string.Empty;
            }

            string[] segments = groupPath.Split('/');
            return segments[^1];
        }
    }
}
#endif
