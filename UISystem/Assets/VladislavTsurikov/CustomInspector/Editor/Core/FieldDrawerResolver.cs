using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace VladislavTsurikov.CustomInspector.Editor.Core
{
    public static class FieldDrawerResolver<TDrawer> where TDrawer : FieldDrawer
    {
        private static readonly List<TDrawer> _fieldDrawers = new();

        static FieldDrawerResolver() => RegisterDrawers();

        private static void RegisterDrawers()
        {
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

            foreach (Assembly assembly in assemblies)
            {
                Type[] drawerTypes = assembly.GetTypes()
                    .Where(t => typeof(TDrawer).IsAssignableFrom(t) && !t.IsAbstract && t.IsClass)
                    .ToArray();

                foreach (Type drawerType in drawerTypes)
                {
                    var instance = (TDrawer)Activator.CreateInstance(drawerType);
                    _fieldDrawers.Add(instance);
                }
            }
        }

        public static TDrawer GetFieldDrawer(Type fieldType) =>
            _fieldDrawers.FirstOrDefault(drawer => drawer.CanDraw(fieldType));
    }
}
