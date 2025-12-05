using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.Core.Runtime;

namespace VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Utility
{
    public static class GroupUtility
    {
#if UNITY_EDITOR
        internal static Group CreateGroup(Type prototypeType)
        {
            Directory.CreateDirectory(MegaWorldPath.PathToGroup);

            var path = string.Empty;

            path += "Group.asset";

            path = CommonPath.CombinePath(MegaWorldPath.PathToGroup, path);
            path = AssetDatabase.GenerateUniqueAssetPath(path);

            Group asset = ScriptableObject.CreateInstance<Group>();

            asset.Init(prototypeType);

            AssetDatabase.CreateAsset(asset, path);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            return asset;
        }
#endif

        public static Type GetGeneralPrototypeType(IReadOnlyList<Group> groups)
        {
            if (groups.Count == 1)
            {
                return groups.Last().PrototypeType;
            }

            Type prototypeType = null;

            for (var i = 0; i < groups.Count; i++)
            {
                if (i == 0)
                {
                    prototypeType = groups[i].PrototypeType;
                }
                else if (prototypeType != groups[i].PrototypeType)
                {
                    return null;
                }
            }

            return prototypeType;
        }
    }
}
