using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes;

namespace VladislavTsurikov.MegaWorld.Runtime.Common.Utility
{
    public static class GetRandomPrototype
    {
        public static Prototype GetMaxSuccessProto(IReadOnlyList<Prototype> protoList)
        {
            if (protoList.Count == 0)
            {
                Debug.Log("You have not selected more than one prototype.");
                return null;
            }

            if (protoList.Count == 1)
            {
                return protoList[0];
            }

            var successProto = new List<float>();

            foreach (Prototype proto in protoList)
            {
                var successSettings = (SuccessSettings)proto.GetElement(typeof(SuccessSettings));

                if (proto.Active == false)
                {
                    successProto.Add(-1);
                    continue;
                }

                var randomSuccess = Random.Range(0.0f, 1.0f);

                if (randomSuccess > successSettings.SuccessValue / 100)
                {
                    randomSuccess = successSettings.SuccessValue / 100;
                }

                successProto.Add(randomSuccess);
            }

            var maxSuccessProto = successProto.Max();

            var maxIndexProto = successProto.IndexOf(maxSuccessProto);
            return protoList[maxIndexProto];
        }

        public static PlacedObjectPrototype GetRandomSelectedPrototype(Group group)
        {
            List<Prototype> protoList = group.GetAllSelectedPrototypes();

            return (PlacedObjectPrototype)protoList[Random.Range(0, protoList.Count)];
        }
    }
}
