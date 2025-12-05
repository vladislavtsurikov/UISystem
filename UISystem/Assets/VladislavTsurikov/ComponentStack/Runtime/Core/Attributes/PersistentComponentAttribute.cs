using System;

namespace VladislavTsurikov.ComponentStack.Runtime.AdvancedComponentStack
{
    /// <summary>
    ///     if you add this attribute to a class component, this component cannot be removed.
    ///     Also, this component will be created itself if somehow this component is not in the collection
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class PersistentComponentAttribute : Attribute
    {
    }
}
