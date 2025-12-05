using System;
using System.Linq;
using System.Reflection;
using VladislavTsurikov.AddressableLoaderSystem.Editor.Core.Create;
using VladislavTsurikov.AddressableLoaderSystem.Runtime.Core;

namespace VladislavTsurikov.AddressableLoaderSystem.Editor.Core
{
    public abstract class ResourceLoaderFieldDescriptorFill : ResourceLoaderDescriptorFill
    {
        public override ResourceLoaderDescriptor Fill(Type loaderType)
        {
            var descriptor = (ResourceLoaderFieldDescriptor)InstanceResourceLoaderDescriptor();
            descriptor.ClassName = loaderType.Name;
            descriptor.Fields.Clear();

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
                var hasAttr = attr != null;
                var address = attr?.Address ?? string.Empty;

                descriptor.Fields.Add(
                    new ResourceLoaderFieldData(member.Name, fieldType, address, hasAttr)
                );
            }

            return descriptor;
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
