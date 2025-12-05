#if UNITY_EDITOR
using System.IO;
using VladislavTsurikov.UIElementsUtility.Content;
using VladislavTsurikov.UIElementsUtility.Editor.Core;
using VladislavTsurikov.UIElementsUtility.Editor.Groups.EditorColors;
using VladislavTsurikov.UIElementsUtility.Runtime.Core.Utility;
using VladislavTsurikov.Utility.Runtime;

namespace VladislavTsurikov.UIElementsUtility.Editor.Groups.SelectableColors
{
    public class DefaultEditorSelectableColorsCreator : DataGroupScriptableObjectCreator<EditorSelectableColorPalette,
        EditorSelectableColorInfo>
    {
        protected override void AddItems(EditorSelectableColorPalette data)
        {
            CreateDefaultEditorColorPaletteIfNecessary();

            data.Items.Clear();

            EditorColorPalette defaultEditorColorPalette =
                DataGroupUtility.GetGroup<EditorColorPalette, EditorColorInfo>("Default");

            foreach (EditorColorInfo colorInfo in defaultEditorColorPalette.Items)
            {
                switch (colorInfo.ColorName)
                {
                    case "Black":
                    case "White":
                    case "Gray":
                        continue;
                    default:
                        data.Items.Add
                        (
                            new EditorSelectableColorInfo
                                {
                                    ColorName =
                                        colorInfo.ColorName.RemoveWhitespaces().RemoveAllSpecialCharacters(),
                                    Normal = new EditorThemeColor
                                    {
                                        ColorOnDark = colorInfo.ThemeColor.ColorOnDark,
                                        ColorOnLight = colorInfo.ThemeColor.ColorOnLight
                                    }
                                }
                                .GenerateAllColorVariantsFromNormalColor()
                        );
                        break;
                }
            }

            data.SortByHue();
        }

        protected override string GetScriptableObjectGenerationPath() =>
            Path.Combine(ContentPath.Path, "SelectableColors");

        private void CreateDefaultEditorColorPaletteIfNecessary()
        {
            EditorColorPalette defaultEditorColorPalette =
                DataGroupUtility.GetGroup<EditorColorPalette, EditorColorInfo>("Default");

            if (defaultEditorColorPalette == null)
            {
                new DefaultEditorColorsCreator().Run();
            }
        }
    }
}
#endif
