using OdinSerializer;
using UnityEngine;

namespace VladislavTsurikov.QuadTree.Runtime
{
    /// <summary>
    ///     An interface that defines and object with a rectangle
    /// </summary>
    public interface IHasRect
    {
        [OdinSerialize]
        Rect Rectangle { get; }
    }
}
