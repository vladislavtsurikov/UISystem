using System.Collections.Generic;

namespace VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Utility
{
    public static class AllAvailableGroups
    {
        private static readonly List<Group> _groupList = new();

        public static List<Group> GetAllGroups() => _groupList;

        public static void AddGroup(Group group)
        {
            if (!_groupList.Contains(group))
            {
                _groupList.Add(group);
            }
        }

        public static void RemoveGroup(Group group) => _groupList.Remove(group);
    }
}
