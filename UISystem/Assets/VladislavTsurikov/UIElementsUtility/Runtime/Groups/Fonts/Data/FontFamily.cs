using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.UIElementsUtility.Runtime.Core;
using VladislavTsurikov.UIElementsUtility.Runtime.Utility;

namespace VladislavTsurikov.UIElementsUtility.Runtime.Groups.Fonts
{
    [
        CreateAssetMenu
        (
            fileName = "FontFamily",
            menuName = "VladislavTsurikov/UIElementsUtility/FontFamily"
        )
    ]
    public class FontFamily : FilesDataGroup<FontFamily, FontInfo>
    {
        public void SortFontsByWeight() => _items = _items.OrderBy(fi => (int)fi.Weight).ToList();

        public Font GetFont(int weightValue) => GetFont((GenericFontWeight)weightValue);

        private Font GetFont(GenericFontWeight weight)
        {
            _items = _items.Where(item => item != null && item.Font != null).ToList();

            foreach (FontInfo fontInfo in _items.Where(fi => fi.Weight == weight))
            {
                return fontInfo.Font;
            }

            Debug.LogWarning($"Font '{AssetDefaultName}-{weight}' not found! Returned default Unity font");

            return ConstantVisualElements.UnityDefaultFont;
        }

#if UNITY_EDITOR
        public override string GetFileFormat() => "ttf";

        protected override void AddItems(string[] files)
        {
            foreach (var filePath in files)
            {
                Font reference = AssetDatabase.LoadAssetAtPath<Font>(filePath);
                if (reference == null)
                {
                    continue;
                }

                _items.Add(new FontInfo { Font = reference });
            }
        }

        internal override void ValidateItems()
        {
            _items = _items.Where(fi => fi != null && fi.Font != null).ToList();
            foreach (FontInfo fontInfo in _items)
            {
                GenericFontWeight weight = GenericFontWeight.Regular;
                foreach (GenericFontWeight style in Enum.GetValues(typeof(GenericFontWeight)))
                {
                    if (!fontInfo.Font.name.Contains($"-{style}"))
                    {
                        continue;
                    }

                    weight = style;
                    break;
                }

                fontInfo.Weight = weight;
            }

            SortFontsByWeight();
        }
#endif
    }
}
