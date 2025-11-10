using System;

namespace VladislavTsurikov.ComponentStack.Runtime.Core
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class DontCreateAttribute : Attribute
    {
    }
}
