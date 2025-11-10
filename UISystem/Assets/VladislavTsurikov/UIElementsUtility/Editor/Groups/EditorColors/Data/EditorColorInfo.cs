#if UNITY_EDITOR
using System;
using UnityEngine;
using VladislavTsurikov.Utility.Runtime;

namespace VladislavTsurikov.UIElementsUtility.Editor.Groups.EditorColors
{
    [Serializable]
    public class EditorColorInfo
    {
        public string ColorName;
        public EditorThemeColor ThemeColor;

        public EditorColorInfo(string colorName, Color colorOnDark, Color colorOnLight)
        {
            ColorName = colorName;
            ValidateName();
            ThemeColor = new EditorThemeColor { ColorOnDark = colorOnDark, ColorOnLight = colorOnLight };
        }

        public EditorColorInfo()
        {
            ColorName = string.Empty;
            ThemeColor = new EditorThemeColor();
        }

        public Color Color => ThemeColor.Color;

        public void ValidateName() => ColorName = ColorName.RemoveWhitespaces().RemoveAllSpecialCharacters();
    }
}
#endif
