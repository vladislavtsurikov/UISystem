#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using VladislavTsurikov.AddressableLoaderSystem.Runtime.Core;
using VladislavTsurikov.ReflectionUtility.Runtime;
using Object = UnityEngine.Object;

namespace VladislavTsurikov.AddressableLoaderSystem.Editor.Core.Warning
{
    [Serializable]
    public class AddressableLoaderValidator
    {
        public static List<ValidationResult> ValidateAll()
        {
            var results = new List<ValidationResult>();

            Type[] loaderTypes = AllTypesDerivedFrom<ResourceLoader>.Types;

            foreach (Type type in loaderTypes)
            {
                ValidationResult result = ValidateLoader(type);
                if (result.MissingAutoLoadFields.Count > 0)
                {
                    results.Add(result);
                    Debug.LogWarning(
                        $"[AddressableLoaderValidator] ResourceLoader '{type.Name}' имеет поля без [AutoLoad]: " +
                        $"{string.Join(", ", result.MissingAutoLoadFields)}");
                }
            }

            if (results.Count == 0)
            {
                Debug.Log(
                    "[AddressableLoaderValidator] Все ResourceLoader корректны, отсутствующие [AutoLoad] не найдены.");
            }

            return results;
        }

        public static ValidationResult ValidateLoader(Type loaderType)
        {
            var result = new ValidationResult { LoaderType = loaderType };

            MemberInfo[] members = loaderType
                .GetMembers(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(m => m.MemberType is MemberTypes.Field or MemberTypes.Property)
                .ToArray();

            foreach (MemberInfo member in members)
            {
                if (member.Name.Contains("k__BackingField"))
                {
                    continue;
                }

                Type memberType = null;

                if (member is FieldInfo field)
                {
                    memberType = field.FieldType;
                }
                else if (member is PropertyInfo prop)
                {
                    memberType = prop.PropertyType;
                }

                if (memberType == null)
                {
                    continue;
                }

                if (!typeof(Object).IsAssignableFrom(memberType))
                {
                    continue;
                }

                var hasAutoLoad = member.GetCustomAttribute<AutoLoadAttribute>() != null;

                if (!hasAutoLoad)
                {
                    result.MissingAutoLoadFields.Add(member.Name);
                }
            }

            return result;
        }

        [Serializable]
        public class ValidationResult
        {
            public List<string> MissingAutoLoadFields = new();
            public Type LoaderType;
        }
    }
}
#endif
