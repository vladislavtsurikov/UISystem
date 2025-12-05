using System;
using UnityEngine.UIElements;

namespace VladislavTsurikov.UIElementsUtility.Runtime.Groups.Layouts
{
    [Serializable]
    public class LayoutInfo
    {
        public VisualTreeAsset UxmlReference;

        public LayoutInfo() => UxmlReference = null;
    }
}
