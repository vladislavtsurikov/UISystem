#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using VladislavTsurikov.AddressableUtility.Editor;
using Object = UnityEngine.Object;

namespace VladislavTsurikov.AddressableLoaderSystem.Editor.Core.Create
{
    [Serializable]
    public class ResourceLoaderFieldData
    {
        private Object _asset;

        public string FieldName { get; private set; }
        public Type FieldType { get; private set; }
        public string Address { get; private set; }
        public bool HasAutoLoadAttribute { get; private set; }

        public Object Asset
        {
            get => _asset;
            set => UpdateFromAsset(value);
        }

        public ResourceLoaderFieldData()
        {
        }

        public ResourceLoaderFieldData(string fieldName, Type fieldType, string address, bool hasAutoLoad)
        {
            FieldName = fieldName;
            FieldType = fieldType;
            Address = address;
            HasAutoLoadAttribute = hasAutoLoad;
            _asset = AddressableEditorExtensions.FindAssetByAddress(address);
        }

        private void UpdateFromAsset(Object newAsset)
        {
            if (newAsset == null)
            {
                _asset = null;
                Address = string.Empty;
                return;
            }

            _asset = newAsset;
            FieldType = newAsset.GetType();

            string assetPath = AssetDatabase.GetAssetPath(newAsset);
            string guid = AssetDatabase.AssetPathToGUID(assetPath);

            AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;
            if (settings != null)
            {
                AddressableAssetEntry entry = settings.FindAssetEntry(guid);
                Address = entry != null ? entry.address : string.Empty;
            }
            else
            {
                Address = string.Empty;
            }

            FieldName = newAsset.name;
        }
    }
}
#endif
