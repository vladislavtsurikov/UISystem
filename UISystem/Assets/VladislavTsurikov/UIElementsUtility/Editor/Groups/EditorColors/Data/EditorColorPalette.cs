#if UNITY_EDITOR
using System.Linq;
using UnityEngine;
using VladislavTsurikov.ColorUtility.Runtime;
using VladislavTsurikov.UIElementsUtility.Runtime.Core;
using VladislavTsurikov.Utility.Runtime;

namespace VladislavTsurikov.UIElementsUtility.Editor.Groups.EditorColors
{
    [
        CreateAssetMenu
        (
            fileName = "EditorColors",
            menuName = "VladislavTsurikov/UIElementsUtility/Color Palette"
        )
    ]
    public class EditorColorPalette : DataGroup<EditorColorPalette, EditorColorInfo>
    {
        public void AddNewItem() => _items.Insert(0, new EditorColorInfo());

        public void SortByColorName() => _items = _items.OrderBy(item => item.ColorName).ToList();

        public void SortByHue() =>
            _items = _items.OrderByDescending(item => item.ThemeColor.ColorOnDark.Hue()).ToList();

        internal Color GetColor(string colorName, bool silent = false)
        {
            var cleanName = colorName.RemoveWhitespaces().RemoveAllSpecialCharacters();

            _items = _items.Where(item => item != null).ToList();

            foreach (EditorColorInfo colorInfo in _items.Where(item => item.ColorName.Equals(cleanName)))
            {
                return colorInfo.Color;
            }

            if (!silent)
            {
                Debug.LogWarning($"Color '{colorName}' not found! Returned Color.magenta");
            }

            return Color.magenta;
        }
    }
}
#endif
