#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEngine;

namespace VladislavTsurikov.AddressableLoaderSystem.Editor.Core.Warning
{
    public static class AddressableLoaderValidator
    {
        public static List<ValidationResult> ValidateAll()
        {
            List<ValidationResult> results = new List<ValidationResult>();

            IReadOnlyList<ResourceLoaderTemplate> templates = EditorResourceLoaderRegistry.Templates;
            for (int i = 0; i < templates.Count; i++)
            {
                ResourceLoaderTemplate template = templates[i];
                if (template == null)
                {
                    Debug.LogWarning("[AddressableLoaderSystem][AddressableLoaderValidator.ValidateAll] ResourceLoaderTemplate entry is null and was skipped.");
                    continue;
                }

                List<string> issues = new List<string>();
                template.Validate(issues);

                if (issues.Count == 0)
                {
                    continue;
                }

                Type loaderType = template.LoaderType;

                ValidationResult result = new ValidationResult
                {
                    LoaderType = loaderType,
                    Issues = issues
                };

                results.Add(result);

                string loaderName = loaderType != null ? loaderType.Name : "Unknown";
                Debug.LogWarning($"[AddressableLoaderValidator] ResourceLoader '{loaderName}' issues: {string.Join(", ", issues)}");
            }

            return results;
        }

        [Serializable]
        public sealed class ValidationResult
        {
            public Type LoaderType;
            public List<string> Issues = new List<string>();
        }
    }
}
#endif
