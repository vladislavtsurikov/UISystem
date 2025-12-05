using System;
using UnityEngine;

namespace VladislavTsurikov.UIElementsUtility.Runtime.Groups.Fonts
{
    [Serializable]
    public class FontInfo
    {
        public Font Font;
        public GenericFontWeight Weight;

        public FontInfo()
        {
            Font = null;
            Weight = GenericFontWeight.Regular;
        }
    }
}
