#if UNITY_EDITOR
using UnityEngine;
using VladislavTsurikov.ColliderSystem.Runtime;
using VladislavTsurikov.MegaWorld.Runtime.Common;
using VladislavTsurikov.MegaWorld.Runtime.Common.Utility;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.PrototypeGameObject;
using VladislavTsurikov.UnityUtility.Runtime;
using GameObjectUtility = VladislavTsurikov.MegaWorld.Runtime.Core.Utility.GameObjectUtility;

namespace VladislavTsurikov.MegaWorld.Editor.PinTool
{
    public static class PlaceObject
    {
        public static PlacedObjectData Place(Group group, RayHit rayHit)
        {
            PlacedObjectPrototype proto = GetRandomPrototype.GetRandomSelectedPrototype(group);

            if (proto == null)
            {
                return null;
            }

            var instance = new Instance(rayHit.Point, Vector3.one, Quaternion.identity);

            GameObject gameObject =
                GameObjectUtility.Instantiate(proto.Prefab, instance.Position, instance.Scale, instance.Rotation);

            if (group.PrototypeType == typeof(PrototypeGameObject))
            {
                group.GetDefaultElement<ContainerForGameObjects>().ParentGameObject(gameObject);
            }

            return new PlacedObjectData(proto, gameObject);
        }
    }
}
#endif
