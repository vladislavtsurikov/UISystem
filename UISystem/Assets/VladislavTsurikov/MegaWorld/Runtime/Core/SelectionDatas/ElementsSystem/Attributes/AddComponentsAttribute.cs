using System;

namespace VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.ElementsSystem
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public abstract class AddComponentsAttribute : Attribute
    {
        public readonly Type[] PrototypeTypes;
        public readonly Type[] Types;

        protected AddComponentsAttribute(Type[] prototypeTypes, Type[] types)
        {
            PrototypeTypes = prototypeTypes;
            Types = types;
        }

        protected AddComponentsAttribute(Type prototypeType, Type[] types)
        {
            PrototypeTypes = new[] { prototypeType };
            Types = types;
        }

        protected AddComponentsAttribute(Type[] types) => Types = types;
    }
}
