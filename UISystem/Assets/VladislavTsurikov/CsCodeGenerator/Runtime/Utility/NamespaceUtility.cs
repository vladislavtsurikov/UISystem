using System;
using System.Collections.Generic;
using System.Linq;

namespace VladislavTsurikov.CsCodeGenerator.Runtime
{
    public static class NamespaceUtility
    {
        public static string GetNamespaceFromPath(string path, string keepAfter, params string[] ignoredFolders)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentException("Path cannot be null or empty", nameof(path));
            }

            var parts = path.Split('/').Select(p => p.Trim()).Where(p => !string.IsNullOrEmpty(p)).ToList();

            // Find the index of the first folder that contains the keepAfter string
            var index = parts.FindIndex(p => p.Contains(keepAfter, StringComparison.OrdinalIgnoreCase));

            // If such a folder was found, keep it and all following folders
            if (index != -1)
            {
                parts = parts.Skip(index).ToList();
            }
            else
                // If not found, clear the parts to return an empty string
            {
                parts = new List<string>();
            }

            parts = parts.Where(p => !ignoredFolders.Contains(p, StringComparer.OrdinalIgnoreCase)).ToList();

            return string.Join(".", parts);
        }

        public static string GetNamespaceFromPath(string path, params string[] ignoredFolders)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentException("Path cannot be null or empty", nameof(path));
            }

            IEnumerable<string> parts = path.Split('/').Select(p => p.Trim()).Where(p => !string.IsNullOrEmpty(p));

            parts = parts.Where(p => !ignoredFolders.Contains(p, StringComparer.OrdinalIgnoreCase));

            return string.Join(".", parts);
        }

        public static List<string> GetUsingDirectives(params Type[] types)
        {
            if (types == null || types.Length == 0)
            {
                throw new ArgumentException("Types cannot be null or empty", nameof(types));
            }

            return types.Select(t => t.Namespace).Distinct().ToList();
        }
    }
}
