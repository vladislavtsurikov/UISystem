using System;

namespace VladislavTsurikov.Nody.Runtime.AdvancedNodeStack
{
    /// <summary>
    ///     if you add this attribute to a node class, this node cannot be removed.
    ///     Also, this node will be created itself if somehow this node is not in the collection
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class PersistentNodeAttribute : Attribute
    {
    }
}
