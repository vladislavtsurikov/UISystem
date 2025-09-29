#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

namespace VladislavTsurikov.ScriptableObjectUtility.Editor
{
    public interface IOnEditorDestroy
    {
        public void OnEditorDestroy();
    }

    public class OnEditorDestroyProcessor : AssetModificationProcessor
    {
        private static readonly Type _type = typeof(IOnEditorDestroy);

        private static readonly string _fileEnding = ".asset";

        public static AssetDeleteResult OnWillDeleteAsset(string path, RemoveAssetOptions _)
        {
            if (!path.EndsWith(_fileEnding))
            {
                return AssetDeleteResult.DidNotDelete;
            }

            Type assetType = AssetDatabase.GetMainAssetTypeAtPath(path);
            if (assetType != null && (assetType.IsSubclassOf(_type) || _type.IsAssignableFrom(assetType)))
            {
                ScriptableObject asset = AssetDatabase.LoadAssetAtPath<ScriptableObject>(path);

                ((IOnEditorDestroy)asset).OnEditorDestroy();
            }

            return AssetDeleteResult.DidNotDelete;
        }
    }
}
#endif
