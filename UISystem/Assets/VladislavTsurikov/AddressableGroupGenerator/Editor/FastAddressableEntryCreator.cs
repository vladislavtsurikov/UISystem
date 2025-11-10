#if ADDRESSABLE_LOADER_SYSTEM_ADDRESSABLES
#if UNITY_EDITOR
using System.Collections.Generic;
using System.Reflection;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;
using UnityEngine.Profiling;

namespace VladislavTsurikov.AddressableGroupGenerator.Editor
{
    public class FastAddressableEntryCreator
    {
        private static readonly ConstructorInfo EntryConstructor = typeof(AddressableAssetEntry)
            .GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, null,
                new[] { typeof(string), typeof(string), typeof(AddressableAssetGroup), typeof(bool) }, null);

        private readonly Dictionary<string, AddressableAssetEntry> _entryMap;
        private readonly FieldInfo _entryMapField;

        private readonly AddressableAssetGroup _group;
        private readonly List<AddressableAssetEntry> _serializeEntries;
        private readonly FieldInfo _serializeEntriesField;

        public FastAddressableEntryCreator(AddressableAssetGroup group)
        {
            _group = group;

            _entryMapField =
                typeof(AddressableAssetGroup).GetField("m_EntryMap", BindingFlags.Instance | BindingFlags.NonPublic);
            _serializeEntriesField = typeof(AddressableAssetGroup).GetField("m_SerializeEntries",
                BindingFlags.Instance | BindingFlags.NonPublic);

            _entryMap = (Dictionary<string, AddressableAssetEntry>)_entryMapField.GetValue(group);
            _serializeEntries = (List<AddressableAssetEntry>)_serializeEntriesField.GetValue(group);
        }

        public void CreateAndAddEntry(string guid, string address, string label, bool assignLabel = true)
        {
            if (_group == null || string.IsNullOrEmpty(guid))
            {
                return;
            }

            if (EntryConstructor == null || _entryMapField == null || _serializeEntriesField == null)
            {
                Debug.LogError("FastAddressableEntryCreator: Cannot access internal Addressables fields.");
                return;
            }

            Profiler.BeginSample("AddressableEntry - Create AddressableAssetEntry");
            var entry = (AddressableAssetEntry)EntryConstructor.Invoke(new object[] { guid, address, _group, false });
            entry.IsSubAsset = false;
            entry.parentGroup = _group;

            if (assignLabel)
            {
                entry.SetLabel(label, true);
            }

            _entryMap[guid] = entry;
            Profiler.EndSample();

            Profiler.BeginSample("AddressableEntry - Add to serializeEntries");
            _serializeEntries.Add(entry);
            Profiler.EndSample();

            _group.SetDirty(AddressableAssetSettings.ModificationEvent.EntryAdded, entry, true, true);
        }
    }
}
#endif
#endif
