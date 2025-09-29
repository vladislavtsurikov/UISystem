namespace VladislavTsurikov.AddressableLoaderSystem.Runtime.Core
{
    /*internal static class AssetBundleGroupTracker
    {
        private static readonly Dictionary<string, (AsyncOperationHandle locations, AsyncOperationHandle download)> _loadedGroups = new();
        private static readonly Dictionary<string, HashSet<ResourceLoader>> _labelOwners = new();

        internal static async UniTask TrackAndLoad(string label, ResourceLoader owner)
        {
            if (!_labelOwners.TryGetValue(label, out var owners))
            {
                owners = new HashSet<ResourceLoader>();
                _labelOwners[label] = owners;

                var locationsHandle = Addressables.LoadResourceLocationsAsync(label);
                await locationsHandle.ToUniTask();

                if (locationsHandle.Result == null || locationsHandle.Result.Count == 0)
                {
                    Debug.LogError($"[AssetBundleGroupTracker] No resources found for label '{label}'.");
                    return;
                }

                var downloadHandle = Addressables.DownloadDependenciesAsync(locationsHandle.Result);
                await downloadHandle.ToUniTask();

                _loadedGroups[label] = (locationsHandle, downloadHandle);
                Debug.Log($"[AssetBundleGroupTracker] Label '{label}' loaded and tracked.");
            }

            if (owners.Add(owner))
            {
                Debug.Log($"[AssetBundleGroupTracker] Label '{label}' is now used by '{owner.GetType().Name}'.");
            }
        }

        internal static void UnloadIfUnused(ResourceLoader owner)
        {
            List<string> labelsToRemove = new();

            foreach (var pair in _labelOwners)
            {
                if (pair.Value.Remove(owner) && pair.Value.Count == 0)
                {
                    if (_loadedGroups.TryGetValue(pair.Key, out var handles))
                    {
                        Addressables.Release(handles.download);
                        Addressables.Release(handles.locations);
                        Debug.Log($"[AssetBundleGroupTracker] Label '{pair.Key}' released.");
                    }

                    _loadedGroups.Remove(pair.Key);
                    labelsToRemove.Add(pair.Key);
                }
            }

            foreach (var label in labelsToRemove)
            {
                _labelOwners.Remove(label);
            }
        }
    }*/
}
