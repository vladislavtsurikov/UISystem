using System;
using System.Collections.Generic;
using OdinSerializer;
using UnityEditor;
using VladislavTsurikov.ScriptableObjectUtility.Runtime;

namespace VladislavTsurikov.AddressableLoaderSystem.Runtime.Core.AddressableLabelMap
{
    [LocationAsset("AddressableLoaderSystem/AddressableLabelMap")]
    public class AddressableLabelMapAsset : SerializedScriptableObjectSingleton<AddressableLabelMapAsset>
    {
        private static List<string> _cachedLabels;

        [OdinSerialize]
        private Dictionary<Type, Dictionary<string, string>> _map = new();

        public bool TryGetLabel(string address, out string label)
        {
            foreach (Dictionary<string, string> entry in _map.Values)
            {
                if (entry.TryGetValue(address, out label))
                {
                    return true;
                }
            }

            label = null;
            return false;
        }

        public Dictionary<string, string> GetLabelsByType(Type type) =>
            _map.TryGetValue(type, out Dictionary<string, string> result) ? result : new Dictionary<string, string>();

        public List<string> GetAllLabels()
        {
            if (_cachedLabels != null)
            {
                return _cachedLabels;
            }

            var allLabels = new HashSet<string>();

            foreach (Dictionary<string, string> typeMap in _map.Values)
            foreach (var label in typeMap.Values)
            {
                allLabels.Add(label);
            }

            _cachedLabels = new List<string>(allLabels);
            return _cachedLabels;
        }

#if UNITY_EDITOR
        public void SetMap(Dictionary<Type, Dictionary<string, string>> newMap)
        {
            _map = newMap;
            EditorUtility.SetDirty(this);
        }
#endif
    }
}
