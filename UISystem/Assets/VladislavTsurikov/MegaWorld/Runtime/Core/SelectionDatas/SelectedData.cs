using System;
using System.Collections.Generic;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.Utility;
using Prototype = VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.Prototype;

namespace VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas
{
    using Prototype = Prototype;

    public class SelectedData
    {
        private List<Group.Group> _selectedGroupList = new();

        private List<SelectedPrototypes> _selectedPrototypeTypes;
        private SelectionData _selectionData;

        public Group.Group SelectedGroup { get; private set; }
        public Prototype SelectedPrototype { get; private set; }
        public IReadOnlyList<Group.Group> SelectedGroupList => _selectedGroupList;

        public IReadOnlyList<Prototype> SelectedPrototypeList
        {
            get
            {
                var prototypeListTest = new List<Prototype>();

                foreach (SelectedPrototypes selectedPrototypeType in _selectedPrototypeTypes)
                {
                    prototypeListTest.AddRange(selectedPrototypeType.SelectedPrototypeList);
                }

                return prototypeListTest;
            }
        }

        public void Setup(SelectionData selectionData)
        {
            _selectionData = selectionData;
            SetupSelectedPrototypeTypes();
        }

        public void SetSelectedData()
        {
            ClearSelectedList();
            SetSelectedList(_selectionData.GroupList);
            SetSelected();
        }

        private void ClearSelectedList()
        {
            _selectedGroupList.Clear();

            foreach (SelectedPrototypes item in _selectedPrototypeTypes)
            {
                item.SelectedPrototypeList.Clear();
            }
        }

        private void SetSelected()
        {
            SetSelectedGroup();
            SetSelectedPrototype();
        }

        private void SetSelectedList(List<Group.Group> groupList)
        {
            for (var index = 0; index < groupList.Count; index++)
            {
                if (groupList[index].Selected)
                {
                    Group.Group selectedGroup = groupList[index];
                    _selectedGroupList.Add(selectedGroup);

                    foreach (SelectedPrototypes item in _selectedPrototypeTypes)
                    {
                        if (item.PrototypeType == selectedGroup.PrototypeType)
                        {
                            SetSelectedPrototypeListFromAssets(selectedGroup.PrototypeList, item.SelectedPrototypeList);
                        }
                    }
                }
            }
        }

        private void SetSelectedGroup()
        {
            if (_selectedGroupList.Count == 1)
            {
                SelectedGroup = _selectedGroupList[SelectedGroupList.Count - 1];
            }
            else
            {
                SelectedGroup = null;
            }
        }

        private void SetSelectedPrototype()
        {
            if (SelectedPrototypeList.Count == 1)
            {
                SelectedPrototype = SelectedPrototypeList[SelectedPrototypeList.Count - 1];
            }
            else
            {
                SelectedPrototype = null;
            }
        }

        public bool HasOneSelectedGroup() => SelectedGroup != null;

        public bool HasOneSelectedPrototype() => SelectedPrototype != null;

        private static void SetSelectedPrototypeListFromAssets<T>(IReadOnlyList<T> baseList, List<T> setPrototypeList)
            where T : Prototype
        {
            foreach (T asset in baseList)
            {
                if (asset == null)
                {
                    continue;
                }

                if (asset.Selected)
                {
                    setPrototypeList.Add(asset);
                }
            }
        }

        public List<Prototype> GetSelectedPrototypes(Type prototypeType)
        {
            foreach (SelectedPrototypes item in _selectedPrototypeTypes)
            {
                if (item.PrototypeType == prototypeType)
                {
                    return item.SelectedPrototypeList;
                }
            }

            return null;
        }

        private void SetupSelectedPrototypeTypes()
        {
            _selectedPrototypeTypes = new List<SelectedPrototypes>();
            _selectedGroupList = new List<Group.Group>();

            foreach (Type type in AllPrototypeTypes.TypeList)
            {
                _selectedPrototypeTypes.Add(new SelectedPrototypes(type));
            }
        }
    }
}
