using System.Collections.Generic;
using UnityEngine;
using VladislavTsurikov.GameObjectCollider.Editor;
using VladislavTsurikov.RendererStack.Editor.Sectorize.GameObjectColliderIntegration;
using VladislavTsurikov.UnityUtility.Runtime;
using GameObjectColliderUtility = VladislavTsurikov.GameObjectCollider.Editor.GameObjectColliderUtility;
#if UNITY_EDITOR
#endif

namespace VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.PrototypeGameObject
{
    public static class UnspawnGameObject
    {
        public static void Unspawn(IReadOnlyList<Prototype> unspawnPrototypes, bool unspawnSelected)
        {
            var unspawnPrefabs = new List<GameObject>();

            foreach (Prototype proto in unspawnPrototypes)
            {
                if (unspawnSelected)
                {
                    if (proto.Selected == false)
                    {
                        continue;
                    }
                }

                unspawnPrefabs.Add((GameObject)proto.PrototypeObject);
            }

            GameObjectUtility.Unspawn(unspawnPrefabs);

#if UNITY_EDITOR
            GameObjectColliderUtility.RemoveNullObjectNodesForAllScenes();
#endif
        }
    }
}
