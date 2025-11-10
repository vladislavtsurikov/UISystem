#if ADDRESSABLE_LOADER_SYSTEM_ADDRESSABLES
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using Cysharp.Threading.Tasks;
using OdinSerializer.Utilities;
using UnityEngine;
using UnityEngine.AddressableAssets;
using VladislavTsurikov.ReflectionUtility.Runtime;
using Object = UnityEngine.Object;

namespace VladislavTsurikov.AddressableLoaderSystem.Runtime.Core
{
    internal static class AssetReferenceReflectionLoader
    {
        private const int MaxDepth = 50;

        internal static async UniTask LoadAssetReferencesRecursive(object result, ResourceLoader owner,
            CancellationToken token)
        {
            var visited = new HashSet<object>(ReferenceEqualityComparer<object>.Default);
            var path = new Stack<object>();
            await LoadAssetReferencesRecursive(result, owner, visited, 0, path, token);
        }

        private static async UniTask LoadAssetReferencesRecursive(
            object result,
            ResourceLoader owner,
            HashSet<object> visited,
            int depth,
            Stack<object> path,
            CancellationToken token)
        {
            if (result is ScriptableObject so)
            {
                await LoadAllAssetReferences(so, owner, visited, depth, path, token);
            }
            else if (result is GameObject go)
            {
                MonoBehaviour[] components = go.GetComponentsInChildren<MonoBehaviour>(true);
                foreach (MonoBehaviour mb in components)
                {
                    await LoadAllAssetReferences(mb, owner, visited, depth, path, token);
                }
            }
        }

        private static async UniTask LoadAllAssetReferences(
            object target,
            ResourceLoader owner,
            HashSet<object> visited,
            int depth,
            Stack<object> path,
            CancellationToken token)
        {
            if (target == null || target is string)
            {
                return;
            }

            if (depth > MaxDepth)
            {
                LogDepthExceeded(path, target);
                return;
            }

            if (!visited.Add(target))
            {
                LogCycleDetected(path, target);
                return;
            }

            if (target.GetType().IsValueType)
            {
                return;
            }

            path.Push(target);

            Type type = target.GetType();
            FieldInfo[] fields = ReflectionFieldCache.GetCachedFields(type,
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            var tasks = new List<UniTask>();

            foreach (FieldInfo field in fields)
            {
                var value = field.GetValue(target);

                if (ShouldSkipField(field, value))
                {
                    continue;
                }

                if (value is AssetReference assetRef && assetRef.RuntimeKeyIsValid())
                {
                    tasks.Add(LoadAndProcess(assetRef, field.FieldType, owner, visited, depth + 1, path, token));
                    continue;
                }

                if (IsEnumerableButNotStringOrTransform(field, value))
                {
                    tasks.Add(ProcessEnumerable(value as IEnumerable, owner, visited, depth + 1, path, token));
                    continue;
                }

                if (IsComplexObject(field))
                {
                    tasks.Add(LoadAllAssetReferences(value, owner, visited, depth + 1, path, token));
                }
            }

            await UniTask.WhenAll(tasks);

            path.Pop();
        }

        private static async UniTask ProcessEnumerable(
            IEnumerable enumerable,
            ResourceLoader owner,
            HashSet<object> visited,
            int depth,
            Stack<object> path,
            CancellationToken token)
        {
            var tasks = new List<UniTask>();

            foreach (var element in enumerable)
            {
                if (element == null)
                {
                    continue;
                }

                Type type = element.GetType();

                if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(KeyValuePair<,>))
                {
                    try
                    {
                        dynamic kv = element;
                        var val = kv.Value;

                        if (val is AssetReference kvAssetRef)
                        {
                            tasks.Add(LoadAndProcess(kvAssetRef, kvAssetRef.GetType(), owner, visited, depth, path,
                                token));
                        }
                        else
                        {
                            tasks.Add(LoadAllAssetReferences(val, owner, visited, depth, path, token));
                        }
                    }
                    catch
                    {
                        // ignore
                    }

                    continue;
                }

                if (element is AssetReference elemAssetRef)
                {
                    tasks.Add(LoadAndProcess(elemAssetRef, type, owner, visited, depth, path, token));
                }
                else
                {
                    tasks.Add(LoadAllAssetReferences(element, owner, visited, depth, path, token));
                }
            }

            await UniTask.WhenAll(tasks);
        }

        private static async UniTask LoadAndProcess(
            AssetReference assetRef,
            Type declaredFieldType,
            ResourceLoader owner,
            HashSet<object> visited,
            int depth,
            Stack<object> path,
            CancellationToken token)
        {
            Object result = await AddressableAssetTracker.TrackAndLoad<Object>(assetRef, owner, token);

            if (IsGenericAssetReferenceOf(declaredFieldType, typeof(ScriptableObject)) && result is ScriptableObject so)
            {
                await LoadAllAssetReferences(so, owner, visited, depth, path, token);
            }
            else if (IsGenericAssetReferenceOf(declaredFieldType, typeof(GameObject)) && result is GameObject go)
            {
                await LoadAssetReferencesRecursive(go, owner, visited, depth, path, token);
            }
        }

        private static bool IsGenericAssetReferenceOf(Type declaredType, Type targetGeneric)
        {
            while (declaredType != null && declaredType != typeof(object))
            {
                if (declaredType.IsGenericType &&
                    declaredType.GetGenericTypeDefinition() == typeof(AssetReferenceT<>))
                {
                    Type genericArg = declaredType.GetGenericArguments()[0];
                    return targetGeneric.IsAssignableFrom(genericArg);
                }

                declaredType = declaredType.BaseType;
            }

            return false;
        }

        private static bool ShouldSkipField(FieldInfo field, object value) =>
            value == null ||
            field.IsDefined(typeof(IgnoreResourceAutoload), true) ||
            (value is Object uObj && uObj == null);

        private static bool IsEnumerableButNotStringOrTransform(FieldInfo field, object value) =>
            field.FieldType != typeof(string) &&
            value is not Transform &&
            value is IEnumerable;

        private static bool IsComplexObject(FieldInfo field)
        {
            Type fieldType = field.FieldType;

            return !fieldType.IsPrimitive &&
                   fieldType != typeof(string) &&
                   !typeof(IEnumerable).IsAssignableFrom(fieldType) &&
                   !typeof(Object).IsAssignableFrom(fieldType);
        }

        private static void LogDepthExceeded(Stack<object> path, object target)
        {
#if ADDRESSABLE_LOADER_LOGS
            string message =
 $"[AssetReferenceLoader] Max depth exceeded while processing: {target.GetType().Name}\nPath:\n{string.Join("\n", path)}";
            Debug.LogError(message);
#endif
        }

        private static void LogCycleDetected(Stack<object> path, object target)
        {
#if ADDRESSABLE_LOADER_LOGS
            string message =
 $"[AssetReferenceLoader] Detected cycle (object already visited): {target.GetType().Name}\nPath:\n{string.Join("\n", path)}";
            Debug.LogError(message);
#endif
        }
    }
}
#endif
