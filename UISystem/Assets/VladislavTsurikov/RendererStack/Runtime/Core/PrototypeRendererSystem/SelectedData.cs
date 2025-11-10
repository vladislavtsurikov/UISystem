using System.Collections.Generic;
using System.Linq;

namespace VladislavTsurikov.RendererStack.Runtime.Core.PrototypeRendererSystem
{
    public class SelectedData
    {
        public List<Group> SelectedGroupList = new();
        public List<Prototype> SelectedProtoList = new();

        public void RefreshSelectedParameters(SelectionData selectionData)
        {
            ClearSelectedList();
            SetSelectedList(selectionData);
        }

        private void ClearSelectedList()
        {
            SelectedProtoList.Clear();
            SelectedGroupList.Clear();
        }

        private void SetSelectedList(SelectionData selectionData)
        {
            foreach (Group group in selectionData.GroupList)
            {
                if (group.Selected)
                {
                    SelectedGroupList.Add(group);
                }
            }

            foreach (Prototype proto in selectionData.PrototypeList)
            {
                if (proto.Selected)
                {
                    SelectedProtoList.Add(proto);
                }
            }
        }

        public bool HasOneSelectedProto() => SelectedProtoList.Count == 1;

        public bool HasOneSelectedGroup() => SelectedGroupList.Count == 1;

        public Prototype GetLastPrototype() => SelectedProtoList.Last();

        public Group GetLastGroup()
        {
            if (SelectedGroupList.Count == 0)
            {
                return null;
            }

            return SelectedGroupList.Last();
        }
    }
}
