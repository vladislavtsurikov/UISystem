//.........................
//.....Generated File......
//.........................
//.......Do not edit.......
//.........................

using UnityEngine;
using VladislavTsurikov.UIElementsUtility.Runtime.Core.Utility;
using VladislavTsurikov.UIElementsUtility.Runtime.Groups.Fonts;

namespace VladislavTsurikov.UIElementsUtility.Runtime
{
    public static class GetFont
    {
        public static class Inter
        {
            public enum FontWeight
            {
                Thin = 0,
                ExtraLight = 1,
                Light = 2,
                Regular = 3,
                Medium = 4,
                SemiBold = 5,
                Bold = 6,
                ExtraBold = 7,
                Black = 8
            }

            private static FontFamily s_fontFamily;
            private static Font s_thin;
            private static Font s_extraLight;
            private static Font s_light;
            private static Font s_regular;
            private static Font s_medium;
            private static Font s_semiBold;
            private static Font s_bold;
            private static Font s_extraBold;
            private static Font s_black;

            private static FontFamily FontFamily => s_fontFamily != null
                ? s_fontFamily
                : s_fontFamily = DataGroupUtility.GetGroup<FontFamily, FontInfo>("Inter");

            public static Font Thin => s_thin ? s_thin : s_thin = GetFont(FontWeight.Thin);

            public static Font ExtraLight =>
                s_extraLight ? s_extraLight : s_extraLight = GetFont(FontWeight.ExtraLight);

            public static Font Light => s_light ? s_light : s_light = GetFont(FontWeight.Light);

            public static Font Regular => s_regular ? s_regular : s_regular = GetFont(FontWeight.Regular);

            public static Font Medium => s_medium ? s_medium : s_medium = GetFont(FontWeight.Medium);

            public static Font SemiBold => s_semiBold ? s_semiBold : s_semiBold = GetFont(FontWeight.SemiBold);

            public static Font Bold => s_bold ? s_bold : s_bold = GetFont(FontWeight.Bold);

            public static Font ExtraBold => s_extraBold ? s_extraBold : s_extraBold = GetFont(FontWeight.ExtraBold);

            public static Font Black => s_black ? s_black : s_black = GetFont(FontWeight.Black);

            private static Font GetFont(FontWeight weight) => FontFamily.GetFont((int)weight);
        }

        public static class Ubuntu
        {
            public enum FontWeight
            {
                Light = 2,
                Regular = 3,
                Medium = 4,
                Bold = 6
            }

            private static FontFamily s_fontFamily;
            private static Font s_light;
            private static Font s_regular;
            private static Font s_medium;
            private static Font s_bold;

            private static FontFamily FontFamily => s_fontFamily != null
                ? s_fontFamily
                : s_fontFamily = DataGroupUtility.GetGroup<FontFamily, FontInfo>("Ubuntu");

            public static Font Light => s_light ? s_light : s_light = GetFont(FontWeight.Light);

            public static Font Regular => s_regular ? s_regular : s_regular = GetFont(FontWeight.Regular);

            public static Font Medium => s_medium ? s_medium : s_medium = GetFont(FontWeight.Medium);

            public static Font Bold => s_bold ? s_bold : s_bold = GetFont(FontWeight.Bold);

            private static Font GetFont(FontWeight weight) => FontFamily.GetFont((int)weight);
        }
    }
}
