using System;

namespace VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.DefaultComponentsSystem
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class AddDefaultGroupComponentsAttribute : Attribute
    {
        public readonly Type[] Types;

        public AddDefaultGroupComponentsAttribute(Type[] types) => Types = types;
    }
}
