using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace VladislavTsurikov.CustomInspector.Editor.Core
{
    public static class FieldDrawerResolver<TDrawer> where TDrawer : FieldDrawer
    {
        private static readonly List<FieldDrawerMatcher<TDrawer>> _matchers = new();

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
                }
            }
        }

        public static TDrawer CreateDrawer(Type fieldType)
        {
            foreach (FieldDrawerMatcher<TDrawer> matcher in _matchers)
            {
                if (!matcher.CanDraw(fieldType))
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

            return null;
        }
    }
}
