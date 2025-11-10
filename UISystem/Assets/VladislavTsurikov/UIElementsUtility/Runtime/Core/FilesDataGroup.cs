using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace VladislavTsurikov.UIElementsUtility.Runtime.Core
{
    public abstract class FilesDataGroup<T, N> : DataGroup<T, N> where T : ScriptableObject
    {
#if UNITY_EDITOR
        internal override void SetupDataGroup() => LoadFilesFromFolderInternal();

        internal void LoadFilesFromFolderInternal()
        {
            _items ??= new List<N>();

            _items.Clear();

            AddItems(GetFiles());
        }

        public string[] GetFiles()
        {
            var assetPath = AssetDatabase.GetAssetPath(this);
            var assetParentFolderPath = assetPath.Replace($"{name}.asset", "");
            return Directory.GetFiles(assetParentFolderPath, "*." + GetFileFormat(), SearchOption.AllDirectories);
        }

        protected virtual void AddItems(string[] files)
        {
        }

        public abstract string GetFileFormat();
#endif
    }
}
