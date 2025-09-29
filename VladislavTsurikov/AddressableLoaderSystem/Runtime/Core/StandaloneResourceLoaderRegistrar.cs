using System;
using System.Collections.Generic;
using System.Linq;
using VladislavTsurikov.ReflectionUtility.Runtime;

namespace VladislavTsurikov.AddressableLoaderSystem.Runtime.Core
{
    public class StandaloneResourceLoaderRegistrar : ResourceLoaderRegistrar
    {
        public override IEnumerable<ResourceLoader> GetLoaders()
        {
            IEnumerable<Type> allTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(x => x.GetTypes())
                .Where(t =>
                    t.IsClass &&
                    !t.IsAbstract &&
                    typeof(ResourceLoader).IsAssignableFrom(t) &&
                    t.HasParameterlessConstructor());

            foreach (Type type in allTypes)
            {
                yield return (ResourceLoader)Activator.CreateInstance(type);
            }
        }
    }
}
