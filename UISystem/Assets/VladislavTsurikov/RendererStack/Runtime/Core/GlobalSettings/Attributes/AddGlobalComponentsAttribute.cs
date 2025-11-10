using System;

namespace VladislavTsurikov.RendererStack.Runtime.Core.GlobalSettings
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class AddGlobalComponentsAttribute : Attribute
    {
        public readonly Type[] Types;

        public AddGlobalComponentsAttribute(Type[] types) => Types = types;
    }
}
