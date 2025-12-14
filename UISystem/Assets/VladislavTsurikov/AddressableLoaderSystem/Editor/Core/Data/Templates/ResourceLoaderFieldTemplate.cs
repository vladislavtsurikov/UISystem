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
        public List<FieldData> Fields  = new();

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

                Fields.Add(new FieldData(member.Name, fieldType, member.Name));
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
