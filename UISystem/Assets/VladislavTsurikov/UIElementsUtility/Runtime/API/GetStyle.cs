//.........................
//.....Generated File......
//.........................
//.......Do not edit.......
//.........................

using UnityEngine.UIElements;
using VladislavTsurikov.UIElementsUtility.Runtime.Core.Utility;
using VladislavTsurikov.UIElementsUtility.Runtime.Groups.Styles;

namespace VladislavTsurikov.UIElementsUtility.Runtime
{
    public static class GetStyle
    {
        public static class VisualElements
        {
            public enum StyleName
            {
                Button,
                ListView,
                ListViewItem
            }

            private static StyleGroup s_styleGroup;
            private static StyleSheet s_button;
            private static StyleSheet s_listView;
            private static StyleSheet s_listViewItem;

            private static StyleGroup StyleGroup => s_styleGroup != null
                ? s_styleGroup
                : s_styleGroup = DataGroupUtility.GetGroup<StyleGroup, StyleInfo>("VisualElements");

            public static StyleSheet Button => s_button ? s_button : s_button = GetStyleSheet(StyleName.Button);

            public static StyleSheet ListView =>
                s_listView ? s_listView : s_listView = GetStyleSheet(StyleName.ListView);

            public static StyleSheet ListViewItem =>
                s_listViewItem ? s_listViewItem : s_listViewItem = GetStyleSheet(StyleName.ListViewItem);

            private static StyleSheet GetStyleSheet(StyleName styleName) =>
                StyleGroup.GetStyleSheet(styleName.ToString());
        }
    }
}
