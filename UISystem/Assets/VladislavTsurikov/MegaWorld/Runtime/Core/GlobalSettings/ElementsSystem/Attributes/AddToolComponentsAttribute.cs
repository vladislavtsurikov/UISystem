using System;

namespace VladislavTsurikov.MegaWorld.Runtime.Core.GlobalSettings.ElementsSystem
{
    [AttributeUsage(AttributeTargets.Class)]
    public class AddToolComponentsAttribute : Attribute
    {
        public readonly Type[] Types;

        public AddToolComponentsAttribute(Type[] types) => Types = types;
    }
}
