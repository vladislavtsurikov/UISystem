using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using VladislavTsurikov.UIElementsUtility.Runtime.Core;
using VladislavTsurikov.Utility.Runtime;

namespace VladislavTsurikov.UIElementsUtility.Runtime.Groups.Styles
{
    [
        CreateAssetMenu
        (
            fileName = "StyleGroup",
            menuName = "VladislavTsurikov/UIElementsUtility/Style Group (USS)"
        )
    ]
    public class StyleGroup : FilesDataGroup<StyleGroup, StyleInfo>
    {
        internal void AddNewItem() => _items.Insert(0, new StyleInfo());

        internal void SortByFileName() => _items = _items.OrderBy(item => item.UssReference.name).ToList();

        internal StyleSheet GetStyleSheet(string styleName)
        {
            var cleanName = styleName.RemoveWhitespaces().RemoveAllSpecialCharacters();

            _items = _items.Where(item => item != null && item.UssReference != null).ToList();

            foreach (StyleInfo styleInfo in _items.Where(item =>
                         item.UssReference.name.RemoveWhitespaces().RemoveAllSpecialCharacters().Equals(cleanName)))
            {
                return styleInfo.UssReference;
            }

            Debug.LogWarning($"USS Style '{styleName}' not found! Returned null");
            return null;
        }

#if UNITY_EDITOR
        public override string GetFileFormat() => "uss";

        protected override void AddItems(string[] files)
        {
            foreach (var filePath in files)
            {
                StyleSheet reference = AssetDatabase.LoadAssetAtPath<StyleSheet>(filePath);
                if (reference == null)
                {
                    continue;
                }

                _items.Add(new StyleInfo { UssReference = reference });
            }
        }
#endif
    }
}
