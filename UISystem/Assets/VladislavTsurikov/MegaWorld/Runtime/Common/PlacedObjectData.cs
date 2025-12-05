using UnityEngine;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes;

namespace VladislavTsurikov.MegaWorld.Runtime.Common
{
    public class PlacedObjectData
    {
        public readonly GameObject GameObject;
        public readonly PlacedObjectPrototype Proto;

        public PlacedObjectData(PlacedObjectPrototype proto, GameObject gameObject)
        {
            Proto = proto;
            GameObject = gameObject;
        }
    }
}
