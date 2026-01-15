using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace VladislavTsurikov.CustomInspector.Editor.Core
{
    public static class DecoratorDrawerResolver<TDrawer> where TDrawer : DecoratorDrawer
    {
        private static readonly List<DecoratorDrawerMatcher<TDrawer>> _matchers = new();

        static DecoratorDrawerResolver() => RegisterDrawers();

        private static void RegisterDrawers()
        {
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

            foreach (Assembly assembly in assemblies)
            {
                Type[] matcherTypes = assembly.GetTypes()
                    .Where(t => typeof(DecoratorDrawerMatcher<TDrawer>).IsAssignableFrom(t) && !t.IsAbstract && t.IsClass)
                    .ToArray();

                foreach (Type matcherType in matcherTypes)
                {
                    var instance = (DecoratorDrawerMatcher<TDrawer>)Activator.CreateInstance(matcherType);
                    _matchers.Add(instance);
                }
            }
        }

        public static List<TDrawer> CreateDrawers(FieldInfo field)
        {
            var drawers = new List<TDrawer>();

            foreach (Attribute attribute in field.GetCustomAttributes())
            {
                foreach (DecoratorDrawerMatcher<TDrawer> matcher in _matchers)
                {
                    if (!matcher.CanDraw(attribute))
                    {
                        continue;
                    }

                    Type drawerType = matcher.DrawerType;
                    if (drawerType == null || !typeof(TDrawer).IsAssignableFrom(drawerType))
                    {
                        continue;
                    }

                    var drawer = (TDrawer)Activator.CreateInstance(drawerType);
                    drawer.Initialize(attribute);
                    drawers.Add(drawer);
                    break;
                }
            }

            return drawers;
        }
    }
}
