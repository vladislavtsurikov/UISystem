//.........................
//.....Generated File......
//.........................
//.......Do not edit.......
//.........................

#if UNITY_EDITOR
using UnityEngine;
using VladislavTsurikov.UIElementsUtility.Editor.Groups.EditorColors;
using VladislavTsurikov.UIElementsUtility.Editor.Groups.SelectableColors;
using VladislavTsurikov.UIElementsUtility.Runtime.Core.Utility;

namespace VladislavTsurikov.UIElementsUtility.Runtime
{
    public static class GetSelectableColor
    {
        public static class EditorUI
        {
            public enum ColorName
            {
                ButtonContainer,
                ButtonIcon,
                ButtonText,
                Add,
                Remove
            }

            private static EditorSelectableColorPalette s_selectableColorPalette;
            private static EditorSelectableColorInfo s_buttonContainer;
            private static EditorSelectableColorInfo s_buttonIcon;
            private static EditorSelectableColorInfo s_buttonText;
            private static EditorSelectableColorInfo s_add;
            private static EditorSelectableColorInfo s_remove;

            private static EditorSelectableColorPalette SelectableColorPalette => s_selectableColorPalette != null
                ? s_selectableColorPalette
                : s_selectableColorPalette =
                    DataGroupUtility.GetGroup<EditorSelectableColorPalette, EditorSelectableColorInfo>("EditorUI");

            public static EditorSelectableColorInfo ButtonContainer => s_buttonContainer ??
                                                                       (s_buttonContainer =
                                                                           GetSelectableColorInfo(ColorName
                                                                               .ButtonContainer));

            public static EditorSelectableColorInfo ButtonIcon =>
                s_buttonIcon ?? (s_buttonIcon = GetSelectableColorInfo(ColorName.ButtonIcon));

            public static EditorSelectableColorInfo ButtonText =>
                s_buttonText ?? (s_buttonText = GetSelectableColorInfo(ColorName.ButtonText));

            public static EditorSelectableColorInfo Add => s_add ?? (s_add = GetSelectableColorInfo(ColorName.Add));

            public static EditorSelectableColorInfo Remove =>
                s_remove ?? (s_remove = GetSelectableColorInfo(ColorName.Remove));

            public static Color GetColor(ColorName colorName, SelectionState state) =>
                SelectableColorPalette.GetColor(colorName.ToString(), state);

            public static EditorThemeColor GetThemeColor(ColorName colorName, SelectionState state) =>
                SelectableColorPalette.GetThemeColor(colorName.ToString(), state);

            private static EditorSelectableColorInfo GetSelectableColorInfo(ColorName colorName) =>
                SelectableColorPalette.GetSelectableColorInfo(colorName.ToString());
        }

        public static class Default
        {
            public enum ColorName
            {
                Red,
                Pink,
                Purple,
                DeepPurple,
                Indigo,
                Blue,
                LightBlue,
                Cyan,
                Teal,
                Green,
                LightGreen,
                Lime,
                Yellow,
                Amber,
                Orange,
                DeepOrange
            }

            private static EditorSelectableColorPalette s_selectableColorPalette;
            private static EditorSelectableColorInfo s_red;
            private static EditorSelectableColorInfo s_pink;
            private static EditorSelectableColorInfo s_purple;
            private static EditorSelectableColorInfo s_deepPurple;
            private static EditorSelectableColorInfo s_indigo;
            private static EditorSelectableColorInfo s_blue;
            private static EditorSelectableColorInfo s_lightBlue;
            private static EditorSelectableColorInfo s_cyan;
            private static EditorSelectableColorInfo s_teal;
            private static EditorSelectableColorInfo s_green;
            private static EditorSelectableColorInfo s_lightGreen;
            private static EditorSelectableColorInfo s_lime;
            private static EditorSelectableColorInfo s_yellow;
            private static EditorSelectableColorInfo s_amber;
            private static EditorSelectableColorInfo s_orange;
            private static EditorSelectableColorInfo s_deepOrange;

            private static EditorSelectableColorPalette SelectableColorPalette => s_selectableColorPalette != null
                ? s_selectableColorPalette
                : s_selectableColorPalette =
                    DataGroupUtility.GetGroup<EditorSelectableColorPalette, EditorSelectableColorInfo>("Default");

            public static EditorSelectableColorInfo Red => s_red ?? (s_red = GetSelectableColorInfo(ColorName.Red));

            public static EditorSelectableColorInfo Pink => s_pink ?? (s_pink = GetSelectableColorInfo(ColorName.Pink));

            public static EditorSelectableColorInfo Purple =>
                s_purple ?? (s_purple = GetSelectableColorInfo(ColorName.Purple));

            public static EditorSelectableColorInfo DeepPurple =>
                s_deepPurple ?? (s_deepPurple = GetSelectableColorInfo(ColorName.DeepPurple));

            public static EditorSelectableColorInfo Indigo =>
                s_indigo ?? (s_indigo = GetSelectableColorInfo(ColorName.Indigo));

            public static EditorSelectableColorInfo Blue => s_blue ?? (s_blue = GetSelectableColorInfo(ColorName.Blue));

            public static EditorSelectableColorInfo LightBlue =>
                s_lightBlue ?? (s_lightBlue = GetSelectableColorInfo(ColorName.LightBlue));

            public static EditorSelectableColorInfo Cyan => s_cyan ?? (s_cyan = GetSelectableColorInfo(ColorName.Cyan));

            public static EditorSelectableColorInfo Teal => s_teal ?? (s_teal = GetSelectableColorInfo(ColorName.Teal));

            public static EditorSelectableColorInfo Green =>
                s_green ?? (s_green = GetSelectableColorInfo(ColorName.Green));

            public static EditorSelectableColorInfo LightGreen =>
                s_lightGreen ?? (s_lightGreen = GetSelectableColorInfo(ColorName.LightGreen));

            public static EditorSelectableColorInfo Lime => s_lime ?? (s_lime = GetSelectableColorInfo(ColorName.Lime));

            public static EditorSelectableColorInfo Yellow =>
                s_yellow ?? (s_yellow = GetSelectableColorInfo(ColorName.Yellow));

            public static EditorSelectableColorInfo Amber =>
                s_amber ?? (s_amber = GetSelectableColorInfo(ColorName.Amber));

            public static EditorSelectableColorInfo Orange =>
                s_orange ?? (s_orange = GetSelectableColorInfo(ColorName.Orange));

            public static EditorSelectableColorInfo DeepOrange =>
                s_deepOrange ?? (s_deepOrange = GetSelectableColorInfo(ColorName.DeepOrange));

            public static Color GetColor(ColorName colorName, SelectionState state) =>
                SelectableColorPalette.GetColor(colorName.ToString(), state);

            public static EditorThemeColor GetThemeColor(ColorName colorName, SelectionState state) =>
                SelectableColorPalette.GetThemeColor(colorName.ToString(), state);

            private static EditorSelectableColorInfo GetSelectableColorInfo(ColorName colorName) =>
                SelectableColorPalette.GetSelectableColorInfo(colorName.ToString());
        }
    }
}
#endif
