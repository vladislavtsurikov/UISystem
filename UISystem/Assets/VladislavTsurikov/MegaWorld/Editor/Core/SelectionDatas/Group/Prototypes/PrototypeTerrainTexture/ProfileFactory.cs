#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.MegaWorld.Runtime.Core;

namespace VladislavTsurikov.MegaWorld.Editor.Core.SelectionDatas.Group.Prototypes.PrototypeTerrainTexture
{
    public static class ProfileFactory
    {
        public static TerrainLayer SaveTerrainLayerAsAsset(string textureName, TerrainLayer terrainLayer)
        {
            Directory.CreateDirectory(MegaWorldPath.TerrainLayerStoragePath);

            var path = MegaWorldPath.TerrainLayerStoragePath + "/" + textureName + ".asset";

            path = AssetDatabase.GenerateUniqueAssetPath(path);

            AssetDatabase.CreateAsset(terrainLayer, path);
            AssetDatabase.SaveAssets();

            return AssetDatabase.LoadAssetAtPath<TerrainLayer>(path);
        }
    }
}
#endif
