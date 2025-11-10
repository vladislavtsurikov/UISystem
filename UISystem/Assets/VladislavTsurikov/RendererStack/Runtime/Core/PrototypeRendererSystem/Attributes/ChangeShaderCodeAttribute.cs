using System;

namespace VladislavTsurikov.RendererStack.Runtime.Core.PrototypeRendererSystem
{
    [AttributeUsage(AttributeTargets.Class)]
    public abstract class ChangeShaderCodeAttribute : Attribute
    {
        public abstract void ChangeShaderCode(Prototype proto);
    }
}
