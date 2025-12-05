#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.AttributeUtility.Runtime;
using VladislavTsurikov.UIElementsUtility.Runtime.Core;

namespace VladislavTsurikov.UIElementsUtility.Editor.Core
{
    public abstract class DataGroupScriptableObjectCreator<T, N> where T : DataGroup<T, N>
    {
        public void Run()
        {
            var defaultColorPaletteName = typeof(T).GetAttribute<CreateAssetMenuAttribute>().fileName;
            var assetUserName = GetGroupName();
            var assetName = defaultColorPaletteName + "_" + assetUserName;
            var defaultColorPalettePath = GetDefaultDataGroupPath(assetName);

            T data = AssetDatabase.LoadAssetAtPath<T>(defaultColorPalettePath);
            var defaultPaletteNotFound = data == null;
            if (defaultPaletteNotFound)
            {
                data = ScriptableObject.CreateInstance<T>();
            }

            data._groupName = assetUserName;
            data._assetDefaultName = defaultColorPaletteName;
            data.name = assetName;

            if (defaultPaletteNotFound)
            {
                AssetDatabase.CreateAsset(data, defaultColorPalettePath);
            }

            AddItems(data);

            EditorUtility.SetDirty(data);
            AssetDatabase.SaveAssets();

            AssetDatabase.Refresh();
        }

        private string GetDefaultDataGroupPath(string assetName)
        {
            var folderPath = GetScriptableObjectGenerationPath();

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            var defaultColorPalettePath = Path.Combine(folderPath, assetName + ".asset");
            return defaultColorPalettePath;
        }

        protected virtual string GetGroupName() => "Default";

        protected abstract void AddItems(T data);
        protected abstract string GetScriptableObjectGenerationPath();
    }
}
#endif
