#if UNITY_EDITOR
using System;
using UnityEngine;

namespace VladislavTsurikov.UIElementsUtility.Editor.Groups.EditorColors
{
    [Serializable]
    public class ThemeColor
    {
        public Color ColorOnDark;
        public Color ColorOnLight;


        public ThemeColor() => ColorOnDark = ColorOnLight = Color.white;

        public virtual bool IsDarkTheme { get; set; }

        public Color Color =>
            IsDarkTheme
                ? ColorOnDark
                : ColorOnLight;
    }
}
#endif
