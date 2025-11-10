using System;
using OdinSerializer;

namespace VladislavTsurikov.ReflectionUtility.Runtime
{
    [Serializable]
    public class TypeSelector<TBaseType>
    {
        [OdinSerialize]
        private Type _type;

        public Type Type
        {
            get => _type;
            set
            {
                if (value != null && !IsValidType(value))
                {
                    throw new InvalidOperationException($"Type {value} must inherit from {typeof(TBaseType)}.");
                }

                _type = value;
            }
        }

        private static bool IsValidType(Type type) => typeof(TBaseType).IsAssignableFrom(type);
    }
}
