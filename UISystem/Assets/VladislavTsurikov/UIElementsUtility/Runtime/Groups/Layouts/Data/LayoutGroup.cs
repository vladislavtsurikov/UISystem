using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using VladislavTsurikov.UIElementsUtility.Runtime.Core;
using VladislavTsurikov.Utility.Runtime;

namespace VladislavTsurikov.UIElementsUtility.Runtime.Groups.Layouts
{
    [
        CreateAssetMenu
        (
            fileName = "LayoutGroup",
            menuName = "VladislavTsurikov/UIElementsUtility/Layout Group (UXML)"
        )
    ]
    public class LayoutGroup : FilesDataGroup<LayoutGroup, LayoutInfo>
    {
        internal void AddNewItem() => _items.Insert(0, new LayoutInfo());

        internal void SortByFileName()
        {
            _items = _items.Where(item => item != null && item.UxmlReference != null).ToList();
            _items = _items.OrderBy(item => item.UxmlReference.name).ToList();
        }

        internal VisualTreeAsset GetVisualTreeAsset(string layoutName)
        {
            var cleanName = layoutName.RemoveWhitespaces().RemoveAllSpecialCharacters();

            _items = _items.Where(item => item != null && item.UxmlReference != null).ToList();

            foreach (LayoutInfo layoutInfo in _items.Where(item =>
                         item.UxmlReference.name.RemoveWhitespaces().RemoveAllSpecialCharacters().Equals(cleanName)))
            {
                return layoutInfo.UxmlReference;
            }

            Debug.LogWarning($"UXML Layout '{layoutName}' not found! Returned null");
            return null;
        }

#if UNITY_EDITOR
        public override string GetFileFormat() => "uxml";

        protected override void AddItems(string[] files)
        {
            foreach (var filePath in files)
            {
                VisualTreeAsset reference = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(filePath);
                if (reference == null)
                {
                    continue;
                }

                _items.Add(new LayoutInfo { UxmlReference = reference });
            }
        }
#endif
    }
}
