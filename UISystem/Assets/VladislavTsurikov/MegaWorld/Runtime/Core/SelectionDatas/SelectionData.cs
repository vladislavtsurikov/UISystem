using System;
using System.Collections.Generic;
using VladislavTsurikov.MegaWorld.Editor.Core.SelectionDatas;
using VladislavTsurikov.MegaWorld.Editor.Core.Window;

namespace VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas
{
    [Serializable]
    public class SelectionData
    {
        public List<Group.Group> GroupList = new();

        public SelectedData SelectedData = new();

        public SelectionData() => Setup();

        internal void Setup() => SelectedData.Setup(this);

        public void DeleteNullElements() => GroupList.RemoveAll(group => group == null);

#if UNITY_EDITOR
        public SelectionDataEditor SelectionDataEditor = new();

        public void OnGUI(SelectionDataDrawer selectionDataDrawer, Type toolType) =>
            SelectionDataEditor.OnGUI(this, selectionDataDrawer, toolType);

        public void SaveAllData()
        {
            GroupList.RemoveAll(group => group == null);

            foreach (Group.Group group in GroupList)
            {
                group.Save();
            }
        }
#endif
    }
}
