#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.ShaderKeywordFilter;
using UnityEngine;
using VladislavTsurikov.ReflectionUtility.Runtime;
using VladislavTsurikov.UISystem.Runtime.Core;
using VladislavTsurikov.UISystem.Runtime.Core.Graph;

namespace VladislavTsurikov.UISystem.Editor.Core.Graph
{
    public static class NodeTreeGenerator
    {
        [MenuItem("Tools/UISystem/Generate Node Tree")]
        public static void Generate()
        {
            Type[] allTypes = AllTypesDerivedFrom<UIHandler>.Types;
            Dictionary<Type, Node> typeToNode = CreateNodes(allTypes);
            List<Node> roots = BuildHierarchy(allTypes, typeToNode);

            SaveNodeTree(roots);

            Debug.Log("UI NodeTreeAsset updated successfully.");
        }

        private static Dictionary<Type, Node> CreateNodes(Type[] allTypes)
        {
            var typeToNode = new Dictionary<Type, Node>();

            foreach (Type type in allTypes)
            {
                var node = new Node
                {
                    HandlerType = type,
                    Filters = type
                        .GetCustomAttributes(typeof(FilterAttribute), true)
                        .Cast<FilterAttribute>()
                        .Select(f => f.GetType())
                        .ToList()
                };

                typeToNode[type] = node;
            }

            return typeToNode;
        }

        private static List<Node> BuildHierarchy(Type[] allTypes, Dictionary<Type, Node> map)
        {
            var roots = new List<Node>();

            foreach (Type type in allTypes)
            {
                Node node = map[type];

                ParentUIHandlerAttribute parentAttr = type
                    .GetCustomAttributes(typeof(ParentUIHandlerAttribute), true)
                    .Cast<ParentUIHandlerAttribute>()
                    .FirstOrDefault();

                if (parentAttr != null)
                {
                    if (map.TryGetValue(parentAttr.ParentType, out Node parentNode))
                    {
                        parentNode.Children.Add(node);
                        continue;
                    }

                    Debug.LogWarning(
                        $"[NodeTreeGenerator] Parent type `{parentAttr.ParentType.FullName}` for `{type.FullName}` not found. This node will be skipped from tree.");
                    continue;
                }

                roots.Add(node);
            }

            return roots;
        }

        private static void SaveNodeTree(List<Node> roots)
        {
            var tree = new NodeTree { Roots = roots };
            NodeTreeAsset.Instance.SetTree(tree);
            AssetDatabase.SaveAssets();
        }
    }
}
#endif
