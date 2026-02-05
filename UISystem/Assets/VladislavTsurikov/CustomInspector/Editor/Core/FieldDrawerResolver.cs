using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace VladislavTsurikov.CustomInspector.Editor.Core
{
    public static class FieldDrawerResolver<TDrawer> where TDrawer : FieldDrawer
    {
        private static readonly List<FieldDrawerMatcher<TDrawer>> _matchers = new();
        private static readonly HashSet<Type> _drawerAttributeTypes = new();
        private static readonly Dictionary<Type, FieldInfo> _typeOnlyFields = new();

        static FieldDrawerResolver() => RegisterDrawers();

        private static void RegisterDrawers()
        {
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

            foreach (Assembly assembly in assemblies)
            {
                Type[] matcherTypes = assembly.GetTypes()
                    .Where(t => typeof(FieldDrawerMatcher<TDrawer>).IsAssignableFrom(t) && !t.IsAbstract &&
                                t.IsClass)
                    .ToArray();

                foreach (Type matcherType in matcherTypes)
                {
                    var instance = (FieldDrawerMatcher<TDrawer>)Activator.CreateInstance(matcherType);
                    _matchers.Add(instance);
                    RegisterDrawerAttributes(instance);
                }
            }
        }

        public static TDrawer CreateDrawer(Type fieldType)
        {
            if (fieldType == null)
            {
                return null;
            }

            FieldInfo field = GetTypeOnlyField(fieldType);
            return CreateDrawer(field);
        }

        public static TDrawer CreateDrawer(FieldInfo field)
        {
            if (field == null)
            {
                return null;
            }

            var fieldAttributeTypes = GetFieldDrawerAttributeTypes(field);
            FieldDrawerMatcher<TDrawer> fallbackMatcher = null;

            foreach (FieldDrawerMatcher<TDrawer> matcher in _matchers)
            {
                if (!matcher.CanDraw(field))
                {
                    continue;
                }

                var matcherAttributeTypes = matcher.AttributeTypes;
                if (matcherAttributeTypes == null || matcherAttributeTypes.Count == 0)
                {
                    fallbackMatcher ??= matcher;
                    continue;
                }

                if (!IsExactMatch(matcherAttributeTypes, fieldAttributeTypes))
                {
                    continue;
                }

                Type drawerType = matcher.DrawerType;
                if (drawerType == null || !typeof(TDrawer).IsAssignableFrom(drawerType))
                {
                    continue;
                }

                return (TDrawer)Activator.CreateInstance(drawerType);
            }

            if (fallbackMatcher != null)
            {
                Type drawerType = fallbackMatcher.DrawerType;
                if (drawerType != null && typeof(TDrawer).IsAssignableFrom(drawerType))
                {
                    return (TDrawer)Activator.CreateInstance(drawerType);
                }
            }

            return null;
        }

        private static void RegisterDrawerAttributes(FieldDrawerMatcher<TDrawer> matcher)
        {
            if (matcher == null)
            {
                return;
            }

            var attributeTypes = matcher.AttributeTypes;
            if (attributeTypes == null)
            {
                return;
            }

            foreach (Type attributeType in attributeTypes)
            {
                if (attributeType == null || !typeof(Attribute).IsAssignableFrom(attributeType))
                {
                    continue;
                }

                _drawerAttributeTypes.Add(attributeType);
            }
        }

        private static HashSet<Type> GetFieldDrawerAttributeTypes(FieldInfo field)
        {
            var fieldAttributeTypes = new HashSet<Type>();
            if (_drawerAttributeTypes.Count == 0)
            {
                return fieldAttributeTypes;
            }

            foreach (Attribute attribute in field.GetCustomAttributes())
            {
                var attributeType = attribute.GetType();
                foreach (Type drawerAttributeType in _drawerAttributeTypes)
                {
                    if (drawerAttributeType.IsAssignableFrom(attributeType))
                    {
                        fieldAttributeTypes.Add(drawerAttributeType);
                    }
                }
            }

            return fieldAttributeTypes;
        }

        private static bool IsExactMatch(IReadOnlyList<Type> matcherAttributeTypes, HashSet<Type> fieldAttributeTypes)
        {
            if (matcherAttributeTypes.Count != fieldAttributeTypes.Count)
            {
                return false;
            }

            foreach (Type attributeType in matcherAttributeTypes)
            {
                if (!fieldAttributeTypes.Contains(attributeType))
                {
                    return false;
                }
            }

            return true;
        }

        private static FieldInfo GetTypeOnlyField(Type fieldType)
        {
            if (_typeOnlyFields.TryGetValue(fieldType, out var field))
            {
                return field;
            }

            var holderType = typeof(TypeOnlyField<>).MakeGenericType(fieldType);
            field = holderType.GetField(nameof(TypeOnlyField<int>.Value), BindingFlags.Public | BindingFlags.Static);
            _typeOnlyFields[fieldType] = field;
            return field;
        }

        private static class TypeOnlyField<T>
        {
            public static T Value;
        }
    }
}
