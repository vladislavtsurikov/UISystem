using System;

namespace VladislavTsurikov.RendererStack.Runtime.Core.PrototypeRendererSystem.PrototypeSettings
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class AddPrototypeComponentsAttribute : Attribute
    {
        public readonly Type[] PrototypeSettings;

        public AddPrototypeComponentsAttribute(Type[] prototypeSettings) => PrototypeSettings = prototypeSettings;
    }
}
