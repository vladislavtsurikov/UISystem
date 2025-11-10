using System.Collections.Generic;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.PrototypeGameObject;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.PrototypeTerrainDetail;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.PrototypeTerrainObject;

namespace VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.Utility
{
    public static class UnspawnUtility
    {
        public static void UnspawnGroups(IReadOnlyList<Group> groupList, bool unspawnSelected)
        {
            foreach (Group group in groupList)
            {
                if (group.PrototypeType == typeof(PrototypeTerrainDetail.PrototypeTerrainDetail))
                {
                    UnspawnTerrainDetail.Unspawn(group.PrototypeList, unspawnSelected);
                }
                else if (group.PrototypeType == typeof(PrototypeTerrainObject.PrototypeTerrainObject))
                {
                    UnspawnTerrainObject.Unspawn(group.PrototypeList, unspawnSelected);
                }
                else if (group.PrototypeType == typeof(PrototypeGameObject.PrototypeGameObject))
                {
                    UnspawnGameObject.Unspawn(group.PrototypeList, unspawnSelected);
                }
            }
        }
    }
}
