using System;

namespace VladislavTsurikov.MegaWorld.Editor.Core.SelectionDatas.ResourceController
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ResourceControllerAttribute : Attribute
    {
        public readonly Type PrototypeType;

        public ResourceControllerAttribute(Type prototypeType) => PrototypeType = prototypeType;
    }
}
