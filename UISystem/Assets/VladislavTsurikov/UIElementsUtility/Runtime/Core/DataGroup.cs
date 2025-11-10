using System;
using System.Collections.Generic;
using UnityEngine;
using VladislavTsurikov.AttributeUtility.Runtime;
using VladislavTsurikov.ScriptableObjectUtility.Runtime;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace VladislavTsurikov.UIElementsUtility.Runtime.Core
{
    [Serializable]
    public abstract class DataGroup<T, N> : ScriptableObjectRegistry<T> where T : ScriptableObject
    {
        [SerializeField]
        protected internal string _groupName;

        [SerializeField]
        [HideInInspector]
        protected internal string _assetDefaultName;

        [SerializeField]
        protected List<N> _items = new();

        public List<N> Items => _items;
        public string GroupName => _groupName;
        public string AssetDefaultName => _assetDefaultName;

#if UNITY_EDITOR
        public void Setup()
        {
            _assetDefaultName = typeof(T).GetAttribute<CreateAssetMenuAttribute>().fileName;

            SetupDataGroup();

            Validate();

            EditorUtility.SetDirty(this);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        internal virtual void SetupDataGroup()
        {
        }

        internal void Validate()
        {
            ValidateGroup();
            ValidateItems();
        }

        internal virtual void ValidateGroup()
        {
            var assetPath = AssetDatabase.GetAssetPath(this);

            AssetDatabase.RenameAsset(assetPath, $"{_assetDefaultName}_{_groupName}");
        }

        internal virtual void ValidateItems()
        {
        }
#endif
    }
}
