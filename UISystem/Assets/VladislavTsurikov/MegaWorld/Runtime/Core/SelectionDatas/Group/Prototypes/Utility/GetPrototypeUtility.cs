using System.Collections.Generic;
using UnityEngine;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Utility;

namespace VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.Utility
{
    public static class GetPrototypeUtility
    {
        public static List<T> GetPrototypes<T>(Object obj)
            where T : Prototype
        {
            var prototypes = new List<T>();

            foreach (Group group in AllAvailableGroups.GetAllGroups())
            {
                if (group.PrototypeType != typeof(T))
                {
                    continue;
                }

                foreach (Prototype proto in group.PrototypeList)
                {
                    if (proto.IsSamePrototypeObject(obj))
                    {
                        prototypes.Add((T)proto);
                    }
                }
            }

            return prototypes;
        }
    }
}
