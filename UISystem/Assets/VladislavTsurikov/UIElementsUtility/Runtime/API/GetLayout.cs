//.........................
//.....Generated File......
//.........................
//.......Do not edit.......
//.........................

using UnityEngine.UIElements;
using VladislavTsurikov.UIElementsUtility.Runtime.Core.Utility;
using VladislavTsurikov.UIElementsUtility.Runtime.Groups.Layouts;

namespace VladislavTsurikov.UIElementsUtility.Runtime
{
    public static class GetLayout
    {
        public static class VisualElements
        {
            public enum LayoutName
            {
                Button,
                ListView,
                ListViewItem
            }

            private static LayoutGroup s_layoutGroup;
            private static VisualTreeAsset s_button;
            private static VisualTreeAsset s_listView;
            private static VisualTreeAsset s_listViewItem;

            private static LayoutGroup LayoutGroup => s_layoutGroup != null
                ? s_layoutGroup
                : s_layoutGroup = DataGroupUtility.GetGroup<LayoutGroup, LayoutInfo>("VisualElements");

            public static VisualTreeAsset Button =>
                s_button ? s_button : s_button = GetVisualTreeAsset(LayoutName.Button);

            public static VisualTreeAsset ListView =>
                s_listView ? s_listView : s_listView = GetVisualTreeAsset(LayoutName.ListView);

            public static VisualTreeAsset ListViewItem => s_listViewItem
                ? s_listViewItem
                : s_listViewItem = GetVisualTreeAsset(LayoutName.ListViewItem);

            private static VisualTreeAsset GetVisualTreeAsset(LayoutName layoutName) =>
                LayoutGroup.GetVisualTreeAsset(layoutName.ToString());
        }

        public static class Samples
        {
            public enum LayoutName
            {
                Background,
                Dialogue,
                Inventory,
                InventoryItem,
                MainMenuDocument,
                Speedometer
            }

            private static LayoutGroup s_layoutGroup;
            private static VisualTreeAsset s_background;
            private static VisualTreeAsset s_dialogue;
            private static VisualTreeAsset s_inventory;
            private static VisualTreeAsset s_inventoryItem;
            private static VisualTreeAsset s_mainMenuDocument;
            private static VisualTreeAsset s_speedometer;

            private static LayoutGroup LayoutGroup => s_layoutGroup != null
                ? s_layoutGroup
                : s_layoutGroup = DataGroupUtility.GetGroup<LayoutGroup, LayoutInfo>("Samples");

            public static VisualTreeAsset Background =>
                s_background ? s_background : s_background = GetVisualTreeAsset(LayoutName.Background);

            public static VisualTreeAsset Dialogue =>
                s_dialogue ? s_dialogue : s_dialogue = GetVisualTreeAsset(LayoutName.Dialogue);

            public static VisualTreeAsset Inventory =>
                s_inventory ? s_inventory : s_inventory = GetVisualTreeAsset(LayoutName.Inventory);

            public static VisualTreeAsset InventoryItem => s_inventoryItem
                ? s_inventoryItem
                : s_inventoryItem = GetVisualTreeAsset(LayoutName.InventoryItem);

            public static VisualTreeAsset MainMenuDocument => s_mainMenuDocument
                ? s_mainMenuDocument
                : s_mainMenuDocument = GetVisualTreeAsset(LayoutName.MainMenuDocument);

            public static VisualTreeAsset Speedometer => s_speedometer
                ? s_speedometer
                : s_speedometer = GetVisualTreeAsset(LayoutName.Speedometer);

            private static VisualTreeAsset GetVisualTreeAsset(LayoutName layoutName) =>
                LayoutGroup.GetVisualTreeAsset(layoutName.ToString());
        }
    }
}
