using UnityEngine;
using UnityEngine.UIElements;

namespace VladislavTsurikov.UIElementsUtility.Runtime.Utility
{
    public static class ConstantVisualElements
    {
        public const int FieldLabelFontSize = 9;
        public const int Spacing = 4;
        public const int Spacing2X = Spacing * 2;
        public const int Spacing3X = Spacing * 3;
        public const int Spacing4X = Spacing * 4;
        public const int EndOfLineSpacing = Spacing * 6;
        private static Font _defaultFont;

        public static Font UnityDefaultFont =>
            _defaultFont ? _defaultFont : _defaultFont = new VisualElement().GetStyleUnityFont();

        public static VisualElement EndOfLineBlock => GetSpaceBlock(0, EndOfLineSpacing, "End of Line");
        public static VisualElement SpaceBlock => GetSpaceBlock(Spacing, Spacing);
        public static VisualElement SpaceBlock2X => GetSpaceBlock(Spacing2X, Spacing2X);
        public static VisualElement SpaceBlock3X => GetSpaceBlock(Spacing3X, Spacing3X);
        public static VisualElement SpaceBlock4X => GetSpaceBlock(Spacing4X, Spacing4X);

        public static VisualElement Column =>
            new VisualElement().SetName("Column").SetStyleFlexDirection(FlexDirection.Column);

        public static VisualElement Row =>
            new VisualElement().SetName("Row").SetStyleFlexDirection(FlexDirection.Row);

        public static VisualElement FlexibleSpace =>
            new VisualElement()
                .SetName("Flexible Space")
                .SetStyleFlexGrow(1);

        public static VisualElement GetSpaceBlock(int size, string name = "") => GetSpaceBlock(size, size, name);

        public static VisualElement GetSpaceBlock(int width, int height, string name = "") =>
            new VisualElement()
                .SetName($"{name} Space Block ({width}x{height})")
                .SetStyleWidth(width)
                .SetStyleHeight(height)
                .SetStyleAlignSelf(Align.Center)
                .SetStyleFlexShrink(0);
    }
}
