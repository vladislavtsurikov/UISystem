#if UNITY_EDITOR
using System.Linq;
using UnityEngine;
using VladislavTsurikov.ColorUtility.Runtime;
using VladislavTsurikov.UIElementsUtility.Editor.Groups.EditorColors;
using VladislavTsurikov.UIElementsUtility.Runtime.Core;
using VladislavTsurikov.Utility.Runtime;

namespace VladislavTsurikov.UIElementsUtility.Editor.Groups.SelectableColors
{
    [
        CreateAssetMenu
        (
            fileName = "SelectableColorPalette",
            menuName = "VladislavTsurikov/UIElementsUtility/Selectable Color Palette"
        )
    ]
    public class EditorSelectableColorPalette : DataGroup<EditorSelectableColorPalette, EditorSelectableColorInfo>
    {
        internal void AddNewItem() => _items.Insert(0, new EditorSelectableColorInfo());

        internal void SortByColorName() => _items = _items.OrderBy(item => item.ColorName).ToList();

        internal void SortByHue() => _items = _items.OrderByDescending(item => item.Normal.ColorOnDark.Hue()).ToList();

        internal EditorSelectableColorInfo GetSelectableColorInfo(string colorName, bool debuggable = false)
        {
            var cleanName = colorName.RemoveWhitespaces().RemoveAllSpecialCharacters();

            _items = _items.Where(item => item != null).ToList();

            foreach (EditorSelectableColorInfo selectableColorInfo in _items.Where(item =>
                         item.ColorName.Equals(cleanName)))
            {
                return selectableColorInfo;
            }

            if (!debuggable)
            {
                Debug.LogWarning($"SelectableColor '{colorName}' not found! Returned null");
            }

            return null;
        }

        internal Color GetColor(string colorName, SelectionState state)
        {
            EditorSelectableColorInfo editorSelectableColorInfo = GetSelectableColorInfo(colorName);
            if (editorSelectableColorInfo != null)
            {
                return editorSelectableColorInfo.GetColor(state);
            }

            Debug.LogWarning($"SelectableColor '{colorName}' not found! Returned Color.magenta");
            return Color.magenta;
        }

        internal EditorThemeColor GetThemeColor(string colorName, SelectionState state)
        {
            EditorSelectableColorInfo editorSelectableColorInfo = GetSelectableColorInfo(colorName);
            if (editorSelectableColorInfo != null)
            {
                return editorSelectableColorInfo.GetThemeColor(state);
            }

            Debug.LogWarning($"SelectableColor '{colorName}' not found! Returned null");
            return null;
        }
    }
}

#endif
