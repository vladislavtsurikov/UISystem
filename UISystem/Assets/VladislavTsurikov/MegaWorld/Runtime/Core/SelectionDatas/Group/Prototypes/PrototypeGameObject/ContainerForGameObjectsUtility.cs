namespace VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.PrototypeGameObject
{
    public class ContainerForGameObjectsUtility
    {
        public static void DestroyGameObjects<T>(SelectionData data) where T : Prototype
        {
            foreach (Group group in data.GroupList)
            {
                if (group.PrototypeType == typeof(T))
                {
                    group.GetDefaultElement<ContainerForGameObjects>().DestroyGameObjects();
                }
            }
        }
    }
}
