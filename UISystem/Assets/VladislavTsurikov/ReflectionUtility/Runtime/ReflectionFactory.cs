using System;
using System.Collections.Generic;
using System.Linq;

namespace VladislavTsurikov.ReflectionUtility.Runtime
{
    public static class ReflectionFactory
    {
        public static IEnumerable<TType> CreateAllInstances<TType>
            (params object[] dependencies) where TType : class
        {
            return AllTypesDerivedFrom<TType>.Types.CreateInstances<TType>(dependencies);
        }

        public static IEnumerable<TType> CreateInstances<TType>(this IEnumerable<Type> types,
            params object[] dependencies) where TType : class
        {
            return types.Select(type =>
                (TType)Activator.CreateInstance(type, dependencies));
        }
    }
}
