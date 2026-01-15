using UnityEngine;
using VladislavTsurikov.Nody.Runtime.Core;

namespace VladislavTsurikov.UIToolkitUtility.Runtime.ElementStack.IconStack
{
    public interface IShowIcon : ISelectable
    {
#if UNITY_EDITOR
        string Name { get; }
        bool IsRedIcon { get; }

        Texture2D PreviewTexture { get; }
#endif
    }
}
