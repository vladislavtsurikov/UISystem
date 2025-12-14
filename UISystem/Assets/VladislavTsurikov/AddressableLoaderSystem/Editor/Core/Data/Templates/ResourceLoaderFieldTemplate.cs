#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using VladislavTsurikov.AddressableLoaderSystem.Runtime.Core;

namespace VladislavTsurikov.AddressableLoaderSystem.Editor.Core
{
    public abstract class ResourceLoaderFieldTemplate : ResourceLoaderTemplate
    {
        public List<FieldData> Fields = new();

        protected override void OnBuildFrom(Type loaderType)
        {
            Fields.Clear();

            MemberInfo[] members = loaderType
                .GetMembers(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic |
                            BindingFlags.DeclaredOnly)
                .Where(m => (m.MemberType == MemberTypes.Field || m.MemberType == MemberTypes.Property) &&
                            !m.Name.Contains("k__BackingField"))
                .ToArray();

            foreach (MemberInfo member in members)
            {
                Type fieldType = GetMemberType(member);
                if (fieldType == null || !typeof(object).IsAssignableFrom(fieldType))
                {
                    continue;
                }

                AutoLoadAttribute attr = member.GetCustomAttribute<AutoLoadAttribute>();
                bool hasAttr = attr != null;
                string address = attr?.Address ?? string.Empty;

                Fields.Add(new FieldData(member.Name, fieldType, address, hasAttr));
            }
        }

        public override void Validate(List<string> issues)
        {
            if (issues == null)
            {
                return;
            }

            if (Fields == null)
            {
                issues.Add("Fields list is null");
                return;
            }

            for (int i = 0; i < Fields.Count; i++)
            {
                FieldData field = Fields[i];
                if (field == null)
                {
                    continue;
                }

                if (field.Asset == null)
                {
                    issues.Add($"{field.FieldName} ({field.Address})");
                }
            }
        }

        private static Type GetMemberType(MemberInfo member) =>
            member switch
            {
                FieldInfo fi => fi.FieldType,
                PropertyInfo pi => pi.PropertyType,
                _ => null
            };
    }
}
#endif
