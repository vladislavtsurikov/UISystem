using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace VladislavTsurikov.AddressableLoaderSystem.Runtime.Core
{
    public abstract class ResourceLoader
    {
        public abstract UniTask LoadResourceLoader(CancellationToken token);

        protected virtual UniTask UnloadResourceLoader(CancellationToken cancellationToken) => UniTask.CompletedTask;

        public async UniTask Unload(CancellationToken cancellationToken)
        {
            AddressableAssetTracker.UnloadIfUnused(this);

            await UnloadResourceLoader(cancellationToken);
        }

        protected UniTask<T> LoadAndTrack<T>(string key, CancellationToken cancellationToken) where T : Object
        {
#if ADDRESSABLE_LOADER_LOGS_VERBOSE
            var swTrack = System.Diagnostics.Stopwatch.StartNew();
#endif

            return AddressableAssetTracker.TrackAndLoad<T>(key, this, cancellationToken)
                .ContinueWith(result =>
                {
#if ADDRESSABLE_LOADER_LOGS_VERBOSE
                    swTrack.Stop();
                    Debug.Log($"[ResourceLoader] TrackAndLoad<{typeof(T).Name}>('{key}') took {swTrack.Elapsed.ToReadableString()}");

                    var swRecursive = System.Diagnostics.Stopwatch.StartNew();
#endif

                    return AssetReferenceReflectionLoader.LoadAssetReferencesRecursive(result, this, cancellationToken)
#if ADDRESSABLE_LOADER_LOGS_VERBOSE
                        .ContinueWith(() =>
                        {
                            swRecursive.Stop();
                            Debug.Log($"[ResourceLoader] LoadAssetReferencesRecursive<{typeof(T).Name}>('{key}') took {swRecursive.Elapsed.ToReadableString()}");
                            return result;
                        });
#else
                        .ContinueWith(() => result);
#endif
                });
        }

        protected UniTask LoadAssetReferencesRecursive<T>(T result, string key, CancellationToken cancellationToken)
            where T : Object
        {
#if ADDRESSABLE_LOADER_LOGS_VERBOSE
            var swRecursive = System.Diagnostics.Stopwatch.StartNew();
#endif

            return AssetReferenceReflectionLoader.LoadAssetReferencesRecursive(result, this, cancellationToken)
                .ContinueWith(() =>
                {
#if ADDRESSABLE_LOADER_LOGS_VERBOSE
                    swRecursive.Stop();
                    Debug.Log($"[ResourceLoader] LoadAssetReferencesRecursive<{typeof(T).Name}>('{key}') took {swRecursive.Elapsed.ToReadableString()}");
#endif
                    return UniTask.CompletedTask;
                });
        }
    }
}
