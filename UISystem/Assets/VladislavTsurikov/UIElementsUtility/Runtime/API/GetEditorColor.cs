//.........................
//.....Generated File......
//.........................
//.......Do not edit.......
//.........................

#if UNITY_EDITOR
using UnityEngine;
using VladislavTsurikov.UIElementsUtility.Editor.Groups.EditorColors;
using VladislavTsurikov.UIElementsUtility.Runtime.Core.Utility;

namespace VladislavTsurikov.UIElementsUtility.Runtime
{
    public static class GetEditorColor
    {
        public static class EditorUI
        {
            public enum ColorName
            {
                Add,
                Remove,
                Selection,
                TextTitle,
                FieldIcon,
                TextSubtitle,
                TextDescription,
                Background
            }

            private static EditorColorPalette s_colorPalette;
            private static Color? s_add;
            private static Color? s_remove;
            private static Color? s_selection;
            private static Color? s_textTitle;
            private static Color? s_fieldIcon;
            private static Color? s_textSubtitle;
            private static Color? s_textDescription;
            private static Color? s_background;

            private static EditorColorPalette СolorPalette => s_colorPalette != null
                ? s_colorPalette
                : s_colorPalette = DataGroupUtility.GetGroup<EditorColorPalette, EditorColorInfo>("EditorUI");

            public static Color Add => (Color)(s_add ?? (s_add = GetColor(ColorName.Add)));

            public static Color Remove => (Color)(s_remove ?? (s_remove = GetColor(ColorName.Remove)));

            public static Color Selection => (Color)(s_selection ?? (s_selection = GetColor(ColorName.Selection)));

            public static Color TextTitle => (Color)(s_textTitle ?? (s_textTitle = GetColor(ColorName.TextTitle)));

            public static Color FieldIcon => (Color)(s_fieldIcon ?? (s_fieldIcon = GetColor(ColorName.FieldIcon)));

            public static Color TextSubtitle =>
                (Color)(s_textSubtitle ?? (s_textSubtitle = GetColor(ColorName.TextSubtitle)));

            public static Color TextDescription =>
                (Color)(s_textDescription ?? (s_textDescription = GetColor(ColorName.TextDescription)));

            public static Color Background => (Color)(s_background ?? (s_background = GetColor(ColorName.Background)));

            private static Color GetColor(ColorName colorName) => СolorPalette.GetColor(colorName.ToString());
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
                DeepOrange,
                Black,
                White,
                Gray
            }

            private static EditorColorPalette s_colorPalette;
            private static Color? s_red;
            private static Color? s_pink;
            private static Color? s_purple;
            private static Color? s_deepPurple;
            private static Color? s_indigo;
            private static Color? s_blue;
            private static Color? s_lightBlue;
            private static Color? s_cyan;
            private static Color? s_teal;
            private static Color? s_green;
            private static Color? s_lightGreen;
            private static Color? s_lime;
            private static Color? s_yellow;
            private static Color? s_amber;
            private static Color? s_orange;
            private static Color? s_deepOrange;
            private static Color? s_black;
            private static Color? s_white;
            private static Color? s_gray;

            private static EditorColorPalette СolorPalette => s_colorPalette != null
                ? s_colorPalette
                : s_colorPalette = DataGroupUtility.GetGroup<EditorColorPalette, EditorColorInfo>("Default");

            public static Color Red => (Color)(s_red ?? (s_red = GetColor(ColorName.Red)));

            public static Color Pink => (Color)(s_pink ?? (s_pink = GetColor(ColorName.Pink)));

            public static Color Purple => (Color)(s_purple ?? (s_purple = GetColor(ColorName.Purple)));

            public static Color DeepPurple => (Color)(s_deepPurple ?? (s_deepPurple = GetColor(ColorName.DeepPurple)));

            public static Color Indigo => (Color)(s_indigo ?? (s_indigo = GetColor(ColorName.Indigo)));

            public static Color Blue => (Color)(s_blue ?? (s_blue = GetColor(ColorName.Blue)));

            public static Color LightBlue => (Color)(s_lightBlue ?? (s_lightBlue = GetColor(ColorName.LightBlue)));

            public static Color Cyan => (Color)(s_cyan ?? (s_cyan = GetColor(ColorName.Cyan)));

            public static Color Teal => (Color)(s_teal ?? (s_teal = GetColor(ColorName.Teal)));

            public static Color Green => (Color)(s_green ?? (s_green = GetColor(ColorName.Green)));

            public static Color LightGreen => (Color)(s_lightGreen ?? (s_lightGreen = GetColor(ColorName.LightGreen)));

            public static Color Lime => (Color)(s_lime ?? (s_lime = GetColor(ColorName.Lime)));

            public static Color Yellow => (Color)(s_yellow ?? (s_yellow = GetColor(ColorName.Yellow)));

            public static Color Amber => (Color)(s_amber ?? (s_amber = GetColor(ColorName.Amber)));

            public static Color Orange => (Color)(s_orange ?? (s_orange = GetColor(ColorName.Orange)));

            public static Color DeepOrange => (Color)(s_deepOrange ?? (s_deepOrange = GetColor(ColorName.DeepOrange)));

            public static Color Black => (Color)(s_black ?? (s_black = GetColor(ColorName.Black)));

            public static Color White => (Color)(s_white ?? (s_white = GetColor(ColorName.White)));

            public static Color Gray => (Color)(s_gray ?? (s_gray = GetColor(ColorName.Gray)));

            private static Color GetColor(ColorName colorName) => СolorPalette.GetColor(colorName.ToString());
        }
    }
}
#endif
