#if UNITY_EDITOR
using System.IO;
using UnityEngine;
using VladislavTsurikov.UIElementsUtility.Content;
using VladislavTsurikov.UIElementsUtility.Editor.Core;

namespace VladislavTsurikov.UIElementsUtility.Editor.Groups.EditorColors
{
    public class DefaultEditorColorsCreator : DataGroupScriptableObjectCreator<EditorColorPalette, EditorColorInfo>
    {
        protected override void AddItems(EditorColorPalette group)
        {
            group.Items.Clear();

            group.Items.Capacity = 20;

            group.Items.Add(new EditorColorInfo("Amber", new Color(1f, 0.77f, 0f), new Color(0.6f, 0.43f, 0f)));
            group.Items.Add(new EditorColorInfo("Black", Color.black, Color.black));
            group.Items.Add(new EditorColorInfo("White", Color.white, Color.white));
            group.Items.Add(new EditorColorInfo("Blue", new Color(0.38f, 0.57f, 0.98f), new Color(0.23f, 0.33f, 0.6f)));
            group.Items.Add(new EditorColorInfo("Cyan", new Color(0.55f, 0.89f, 1f), new Color(0.29f, 0.46f, 0.45f)));
            group.Items.Add(new EditorColorInfo("Deep Orange", new Color(0.86f, 0.36f, 0.33f),
                new Color(0.55f, 0.18f, 0.05f)));
            group.Items.Add(new EditorColorInfo("Deep Purple", new Color(0.52f, 0.44f, 0.98f),
                new Color(0.22f, 0.16f, 0.6f)));
            group.Items.Add(new EditorColorInfo("Gray", new Color(0.35f, 0.35f, 0.35f),
                new Color(0.54f, 0.54f, 0.54f)));
            group.Items.Add(new EditorColorInfo("Green", new Color(0.55f, 0.89f, 0.51f),
                new Color(0.31f, 0.5f, 0.27f)));
            group.Items.Add(
                new EditorColorInfo("Indigo", new Color(0.44f, 0.53f, 0.98f), new Color(0.22f, 0.27f, 0.6f)));
            group.Items.Add(new EditorColorInfo("Light Blue", new Color(0.44f, 0.7f, 0.99f),
                new Color(0.28f, 0.46f, 0.6f)));
            group.Items.Add(new EditorColorInfo("Light Green", new Color(0.76f, 0.98f, 0.29f),
                new Color(0.38f, 0.5f, 0.15f)));
            group.Items.Add(new EditorColorInfo("Lime", new Color(0.9f, 0.99f, 0.33f), new Color(0.48f, 0.51f, 0.14f)));
            group.Items.Add(new EditorColorInfo("Orange", new Color(0.89f, 0.58f, 0.16f),
                new Color(0.54f, 0.33f, 0.09f)));
            group.Items.Add(new EditorColorInfo("Pink", new Color(0.87f, 0.44f, 0.64f), new Color(0.52f, 0.23f, 0.4f)));
            group.Items.Add(
                new EditorColorInfo("Purple", new Color(0.71f, 0.33f, 0.94f), new Color(0.42f, 0.14f, 0.6f)));
            group.Items.Add(new EditorColorInfo("Red", new Color(0.85f, 0.31f, 0.45f), new Color(0.51f, 0.14f, 0.17f)));
            group.Items.Add(new EditorColorInfo("Teal", new Color(0.56f, 0.9f, 0.73f), new Color(0.31f, 0.51f, 0.4f)));
            group.Items.Add(new EditorColorInfo("Yellow", new Color(0.97f, 0.91f, 0.25f),
                new Color(0.53f, 0.48f, 0.13f)));

            group.SortByHue();
        }

        protected override string GetScriptableObjectGenerationPath() => Path.Combine(ContentPath.Path, "EditorColors");
    }
}
#endif
