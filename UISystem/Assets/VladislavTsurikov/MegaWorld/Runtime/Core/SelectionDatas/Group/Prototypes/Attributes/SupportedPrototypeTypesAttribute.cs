using System;

namespace VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class SupportedPrototypeTypesAttribute : Attribute
    {
        public readonly Type[] PrototypeTypes;

        public SupportedPrototypeTypesAttribute(Type[] prototypeTypes) => PrototypeTypes = prototypeTypes;
    }
}
