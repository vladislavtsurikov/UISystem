namespace VladislavTsurikov.UIElementsUtility.Runtime.Core.Utility
{
    public static class DataGroupUtility
    {
        public static T GetGroup<T, N>(string groupName) where T : DataGroup<T, N>
        {
            foreach (T group in DataGroup<T, N>.AllInstances)
            {
                if (group.GroupName == groupName)
                {
                    return group;
                }
            }

            return null;
        }
    }
}
