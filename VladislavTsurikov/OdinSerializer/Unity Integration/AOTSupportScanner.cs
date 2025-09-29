//-----------------------------------------------------------------------
// <copyright file="AOTSupportScanner.cs" company="Sirenix IVS">
// Copyright (c) 2018 Sirenix IVS
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using OdinSerializer.Utilities;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.SceneManagement;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;

#if UNITY_EDITOR

namespace OdinSerializer.Editor
{
    public sealed class AOTSupportScanner : IDisposable
    {
        private static readonly Stopwatch smartProgressBarWatch = Stopwatch.StartNew();
        private static int smartProgressBarDisplaysSinceLastUpdate;

        private static readonly MethodInfo PlayerSettings_GetPreloadedAssets_Method =
            typeof(PlayerSettings).GetMethod("GetPreloadedAssets", BindingFlags.Public | BindingFlags.Static, null,
                Type.EmptyTypes, null);

        private static readonly PropertyInfo Debug_Logger_Property =
            typeof(Debug).GetProperty("unityLogger") ?? typeof(Debug).GetProperty("logger");

        private static readonly Dictionary<Assembly, bool> IsEditorOnlyAssembly_Cache = new();

        private static readonly HashSet<string> EditorAssemblyNames = new()
        {
            "Assembly-CSharp-Editor",
            "Assembly-UnityScript-Editor",
            "Assembly-Boo-Editor",
            "Assembly-CSharp-Editor-firstpass",
            "Assembly-UnityScript-Editor-firstpass",
            "Assembly-Boo-Editor-firstpass",
            "Sirenix.OdinInspector.Editor",
            "Sirenix.Utilities.Editor",
            "Sirenix.Reflection.Editor",
            typeof(UnityEditor.Editor).Assembly.GetName().Name
        };

        private readonly HashSet<string> scannedPathsNoDependencies = new();
        private readonly HashSet<string> scannedPathsWithDependencies = new();
        private readonly HashSet<Type> seenSerializedTypes = new();

        private bool allowRegisteringScannedTypes;
        private bool scanning;

        public void Dispose()
        {
            if (scanning)
            {
                FormatterLocator.OnLocatedEmittableFormatterForType -= OnLocatedEmitType;
                FormatterLocator.OnLocatedFormatter -= OnLocatedFormatter;
                Serializer.OnSerializedType -= OnSerializedType;

                scanning = false;
                seenSerializedTypes.Clear();
                allowRegisteringScannedTypes = false;
            }
        }

        public void BeginScan()
        {
            scanning = true;
            allowRegisteringScannedTypes = false;

            seenSerializedTypes.Clear();
            scannedPathsNoDependencies.Clear();
            scannedPathsWithDependencies.Clear();

            FormatterLocator.OnLocatedEmittableFormatterForType += OnLocatedEmitType;
            FormatterLocator.OnLocatedFormatter += OnLocatedFormatter;
            Serializer.OnSerializedType += OnSerializedType;
        }

        public bool ScanPreloadedAssets(bool showProgressBar)
        {
            // The API does not exist in this version of Unity
            if (PlayerSettings_GetPreloadedAssets_Method == null)
            {
                return true;
            }

            var assets = (Object[])PlayerSettings_GetPreloadedAssets_Method.Invoke(null, null);

            if (assets == null)
            {
                return true;
            }

            try
            {
                for (var i = 0; i < assets.Length; i++)
                {
                    if (showProgressBar && DisplaySmartUpdatingCancellableProgressBar(
                            "Scanning preloaded assets for AOT support", i + 1 + " / " + assets.Length,
                            (float)i / assets.Length))
                    {
                        return false;
                    }

                    Object asset = assets[i];

                    if (asset == null)
                    {
                        continue;
                    }

                    if (AssetDatabase.Contains(asset))
                    {
                        // Scan the asset and all its dependencies
                        var path = AssetDatabase.GetAssetPath(asset);
                        ScanAsset(path, true);
                    }
                    else
                    {
                        // Just scan the asset
                        ScanObject(asset);
                    }
                }
            }
            finally
            {
                if (showProgressBar)
                {
                    EditorUtility.ClearProgressBar();
                }
            }

            return true;
        }

        public bool ScanAssetBundle(string bundle)
        {
            var assets = AssetDatabase.GetAssetPathsFromAssetBundle(bundle);

            foreach (var asset in assets)
            {
                ScanAsset(asset, true);
            }

            return true;
        }

        public bool ScanAllAssetBundles(bool showProgressBar)
        {
            try
            {
                var bundles = AssetDatabase.GetAllAssetBundleNames();

                for (var i = 0; i < bundles.Length; i++)
                {
                    var bundle = bundles[i];

                    if (showProgressBar &&
                        DisplaySmartUpdatingCancellableProgressBar("Scanning asset bundles for AOT support", bundle,
                            (float)i / bundles.Length))
                    {
                        return false;
                    }

                    ScanAssetBundle(bundle);
                }
            }
            finally
            {
                if (showProgressBar)
                {
                    EditorUtility.ClearProgressBar();
                }
            }

            return true;
        }

        public bool ScanAllAddressables(bool includeAssetDependencies, bool showProgressBar)
        {
            // We don't know whether the addressables package is installed or not. So... needs must.
            // Our only real choice is to utilize reflection that's stocked to the brim with failsafes
            // and error logging.
            //
            // Truly, the code below should not have needed to be written.

            // The following section is the code as it would be without reflection. Please modify this 
            // code reference to be accurate if the reflection code is changed.

            /*

            var settings = UnityEditor.AddressableAssets.AddressableAssetSettingsDefaultObject.Settings;

            if (settings != null && settings.groups != null)
            {
                foreach (AddressableAssetGroup group in settings.groups)
                {
                    if (group.HasSchema(typeof(PlayerDataGroupSchema))) continue;

                    List<AddressableAssetEntry> results = new List<AddressableAssetEntry>();

                    group.GatherAllAssets(results, true, true, true, null);

                    foreach (var result in results)
                    {
                        this.ScanAsset(result.AssetPath, includeAssetDependencies);
                    }
                }
            }

            */

            var progressBarWasDisplayed = false;

            try
            {
                Type AddressableAssetSettingsDefaultObject_Type =
                    TwoWaySerializationBinder.Default.BindToType(
                        "UnityEditor.AddressableAssets.AddressableAssetSettingsDefaultObject");
                if (AddressableAssetSettingsDefaultObject_Type == null)
                {
                    return true;
                }

                PropertyInfo AddressableAssetSettingsDefaultObject_Settings =
                    AddressableAssetSettingsDefaultObject_Type.GetProperty("Settings");
                if (AddressableAssetSettingsDefaultObject_Settings == null)
                {
                    throw new NotSupportedException(
                        "AddressableAssetSettingsDefaultObject.Settings property not found");
                }

                var settings = (ScriptableObject)AddressableAssetSettingsDefaultObject_Settings.GetValue(null, null);

                if (settings == null)
                {
                    return true;
                }

                Type AddressableAssetSettings_Type = settings.GetType();
                PropertyInfo AddressableAssetSettings_groups = AddressableAssetSettings_Type.GetProperty("groups");
                if (AddressableAssetSettings_groups == null)
                {
                    throw new NotSupportedException("AddressableAssetSettings.groups property not found");
                }

                var groups = (IList)AddressableAssetSettings_groups.GetValue(settings, null);

                if (groups == null)
                {
                    return true;
                }

                Type PlayerDataGroupSchema_Type =
                    TwoWaySerializationBinder.Default.BindToType(
                        "UnityEditor.AddressableAssets.Settings.GroupSchemas.PlayerDataGroupSchema");

                Type AddressableAssetGroup_Type = null;
                MethodInfo AddressableAssetGroup_HasSchema = null;
                MethodInfo AddressableAssetGroup_GatherAllAssets = null;

                Type AddressableAssetEntry_Type =
                    TwoWaySerializationBinder.Default.BindToType(
                        "UnityEditor.AddressableAssets.Settings.AddressableAssetEntry");
                if (AddressableAssetEntry_Type == null)
                {
                    throw new NotSupportedException("AddressableAssetEntry type not found");
                }

                Type List_AddressableAssetEntry_Type = typeof(List<>).MakeGenericType(AddressableAssetEntry_Type);
                Type Func_AddressableAssetEntry_bool_Type =
                    typeof(Func<,>).MakeGenericType(AddressableAssetEntry_Type, typeof(bool));
                PropertyInfo AddressableAssetEntry_AssetPath = AddressableAssetEntry_Type.GetProperty("AssetPath");
                if (AddressableAssetEntry_AssetPath == null)
                {
                    throw new NotSupportedException("AddressableAssetEntry.AssetPath property not found");
                }

                foreach (var groupObj in groups)
                {
                    var group = (ScriptableObject)groupObj;
                    if (group == null)
                    {
                        continue;
                    }

                    var groupName = group.name;

                    if (AddressableAssetGroup_Type == null)
                    {
                        AddressableAssetGroup_Type = group.GetType();
                        AddressableAssetGroup_HasSchema = AddressableAssetGroup_Type.GetMethod("HasSchema",
                            Flags.InstancePublic, null, new[] { typeof(Type) }, null);
                        if (AddressableAssetGroup_HasSchema == null)
                        {
                            throw new NotSupportedException(
                                "AddressableAssetGroup.HasSchema(Type type) method not found");
                        }

                        AddressableAssetGroup_GatherAllAssets = AddressableAssetGroup_Type.GetMethod("GatherAllAssets",
                            Flags.InstancePublic, null,
                            new[]
                            {
                                List_AddressableAssetEntry_Type, typeof(bool), typeof(bool), typeof(bool),
                                Func_AddressableAssetEntry_bool_Type
                            }, null);
                        if (AddressableAssetGroup_GatherAllAssets == null)
                        {
                            throw new NotSupportedException(
                                "AddressableAssetGroup.GatherAllAssets(List<AddressableAssetEntry> results, bool includeSelf, bool recurseAll, bool includeSubObjects, Func<AddressableAssetEntry, bool> entryFilter) method not found");
                        }
                    }

                    var hasPlayerDataGroupSchema = false;

                    if (PlayerDataGroupSchema_Type != null)
                    {
                        hasPlayerDataGroupSchema =
                            (bool)AddressableAssetGroup_HasSchema.Invoke(group,
                                new object[] { PlayerDataGroupSchema_Type });
                    }

                    if (hasPlayerDataGroupSchema)
                    {
                        continue; // Skip this group, since it contains all the player data such as resources and build scenes, and we're scanning that separately
                    }

                    var results = (IList)Activator.CreateInstance(List_AddressableAssetEntry_Type);

                    AddressableAssetGroup_GatherAllAssets.Invoke(group,
                        new object[] { results, true, true, true, null });

                    for (var i = 0; i < results.Count; i++)
                    {
                        var entry = results[i];
                        if (entry == null)
                        {
                            continue;
                        }

                        var assetPath = (string)AddressableAssetEntry_AssetPath.GetValue(entry, null);

                        if (showProgressBar)
                        {
                            progressBarWasDisplayed = true;

                            if (DisplaySmartUpdatingCancellableProgressBar("Scanning addressables for AOT support",
                                    groupName + ": " + assetPath, (float)i / results.Count))
                            {
                                return false;
                            }
                        }

                        // Finally!
                        ScanAsset(assetPath, includeAssetDependencies);
                    }
                }
            }
            catch (NotSupportedException ex)
            {
                Debug.LogWarning("Could not AOT scan Addressables assets due to missing APIs: " + ex.Message);
            }
            catch (Exception ex)
            {
                Debug.LogError("Scanning addressables failed with the following exception...");
                Debug.LogException(ex);
            }
            finally
            {
                if (progressBarWasDisplayed)
                {
                    EditorUtility.ClearProgressBar();
                }
            }

            return true;
        }

        public bool ScanAllResources(bool includeResourceDependencies, bool showProgressBar,
            List<string> resourcesPaths = null)
        {
            if (resourcesPaths == null)
            {
                resourcesPaths = new List<string> { "" };
            }

            try
            {
                if (showProgressBar && DisplaySmartUpdatingCancellableProgressBar("Scanning resources for AOT support",
                        "Loading resource assets", 0f))
                {
                    return false;
                }

                var resourcesPathsSet = new HashSet<string>();

                for (var i = 0; i < resourcesPaths.Count; i++)
                {
                    var resourcesPath = resourcesPaths[i];

                    if (showProgressBar && DisplaySmartUpdatingCancellableProgressBar(
                            "Listing resources for AOT support", resourcesPath,
                            (float)i / resourcesPaths.Count))
                    {
                        return false;
                    }

                    Object[] resources = Resources.LoadAll(resourcesPath);

                    foreach (Object resource in resources)
                    {
                        try
                        {
                            var assetPath = AssetDatabase.GetAssetPath(resource);

                            if (assetPath != null)
                            {
                                resourcesPathsSet.Add(assetPath);
                            }
                        }
                        catch (MissingReferenceException ex)
                        {
                            Debug.LogError(
                                "A resource threw a missing reference exception when scanning. Skipping resource and continuing scan.",
                                resource);
                            Debug.LogException(ex, resource);
                        }
                    }
                }

                var resourcePaths = resourcesPathsSet.ToArray();

                for (var i = 0; i < resourcePaths.Length; i++)
                {
                    if (resourcePaths[i] == null)
                    {
                        continue;
                    }

                    try
                    {
                        if (showProgressBar && DisplaySmartUpdatingCancellableProgressBar(
                                "Scanning resource " + i + " for AOT support", resourcePaths[i],
                                (float)i / resourcePaths.Length))
                        {
                            return false;
                        }

                        var assetPath = resourcePaths[i];

                        // Exclude editor-only resources
                        if (assetPath.ToLower().Contains("/editor/"))
                        {
                            continue;
                        }

                        ScanAsset(assetPath, includeResourceDependencies);
                    }
                    catch (MissingReferenceException ex)
                    {
                        Debug.LogError("A resource '" + resourcePaths[i] +
                                       "' threw a missing reference exception when scanning. Skipping resource and continuing scan.");
                        Debug.LogException(ex);
                    }
                }

                return true;
            }
            finally
            {
                if (showProgressBar)
                {
                    EditorUtility.ClearProgressBar();
                }
            }
        }

        public bool ScanBuildScenes(bool includeSceneDependencies, bool showProgressBar)
        {
            var scenePaths = EditorBuildSettings.scenes
                .Where(n => n.enabled)
                .Select(n => n.path)
                .ToArray();

            return ScanScenes(scenePaths, includeSceneDependencies, showProgressBar);
        }

        public bool ScanScenes(string[] scenePaths, bool includeSceneDependencies, bool showProgressBar)
        {
            if (scenePaths.Length == 0)
            {
                return true;
            }

            var formerForceEditorModeSerialization = UnitySerializationUtility.ForceEditorModeSerialization;

            try
            {
                UnitySerializationUtility.ForceEditorModeSerialization = true;

                var hasDirtyScenes = false;

                for (var i = 0; i < EditorSceneManager.sceneCount; i++)
                {
                    if (EditorSceneManager.GetSceneAt(i).isDirty)
                    {
                        hasDirtyScenes = true;
                        break;
                    }
                }

                if (hasDirtyScenes && !EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                {
                    return false;
                }

                SceneSetup[] oldSceneSetup = EditorSceneManager.GetSceneManagerSetup();

                try
                {
                    for (var i = 0; i < scenePaths.Length; i++)
                    {
                        var scenePath = scenePaths[i];

                        if (showProgressBar && DisplaySmartUpdatingCancellableProgressBar(
                                "Scanning scenes for AOT support",
                                "Scene " + (i + 1) + "/" + scenePaths.Length + " - " + scenePath,
                                (float)i / scenePaths.Length))
                        {
                            return false;
                        }

                        if (!File.Exists(scenePath))
                        {
                            Debug.LogWarning("Skipped AOT scanning scene '" + scenePath +
                                             "' for a file not existing at the scene path.");
                            continue;
                        }

                        var openScene = default(Scene);

                        try
                        {
                            openScene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
                        }
                        catch
                        {
                            Debug.LogWarning("Skipped AOT scanning scene '" + scenePath +
                                             "' for throwing exceptions when trying to load it.");
                            continue;
                        }

                        GameObject[] sceneGOs = Resources.FindObjectsOfTypeAll<GameObject>();

                        foreach (GameObject go in sceneGOs)
                        {
                            if (go.scene != openScene)
                            {
                                continue;
                            }

                            if ((go.hideFlags & HideFlags.DontSaveInBuild) == 0)
                            {
                                foreach (ISerializationCallbackReceiver component in go
                                             .GetComponents<ISerializationCallbackReceiver>())
                                {
                                    try
                                    {
                                        allowRegisteringScannedTypes = true;
                                        component.OnBeforeSerialize();

                                        var prefabSupporter = component as ISupportsPrefabSerialization;

                                        if (prefabSupporter != null)
                                        {
                                            // Also force a serialization of the object's prefab modifications, in case there are unknown types in there
                                            List<Object> objs = null;
                                            List<PrefabModification> mods =
                                                UnitySerializationUtility.DeserializePrefabModifications(
                                                    prefabSupporter.SerializationData.PrefabModifications,
                                                    prefabSupporter.SerializationData
                                                        .PrefabModificationsReferencedUnityObjects);
                                            UnitySerializationUtility.SerializePrefabModifications(mods, ref objs);
                                        }
                                    }
                                    finally
                                    {
                                        allowRegisteringScannedTypes = false;
                                    }
                                }
                            }
                        }
                    }

                    // Load a new empty scene that will be unloaded immediately, just to be sure we completely clear all changes made by the scan
                    // Sometimes this fails for unknown reasons. In that case, swallow any exceptions, and just soldier on and hope for the best!
                    // Additionally, also eat any debug logs that happen here, because logged errors can stop the build process, and we don't want
                    // that to happen.

                    UnityEngine.ILogger logger = null;

                    if (Debug_Logger_Property != null)
                    {
                        logger = (UnityEngine.ILogger)Debug_Logger_Property.GetValue(null, null);
                    }

                    var previous = true;

                    try
                    {
                        if (logger != null)
                        {
                            previous = logger.logEnabled;
                            logger.logEnabled = false;
                        }

                        EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
                    }
                    catch
                    {
                    }
                    finally
                    {
                        if (logger != null)
                        {
                            logger.logEnabled = previous;
                        }
                    }
                }
                finally
                {
                    if (oldSceneSetup != null && oldSceneSetup.Length > 0)
                    {
                        if (showProgressBar)
                        {
                            EditorUtility.DisplayProgressBar("Restoring scene setup", "", 1.0f);
                        }

                        EditorSceneManager.RestoreSceneManagerSetup(oldSceneSetup);
                    }
                }

                if (includeSceneDependencies)
                {
                    for (var i = 0; i < scenePaths.Length; i++)
                    {
                        var scenePath = scenePaths[i];
                        if (showProgressBar && DisplaySmartUpdatingCancellableProgressBar(
                                "Scanning scene dependencies for AOT support",
                                "Scene " + (i + 1) + "/" + scenePaths.Length + " - " + scenePath,
                                (float)i / scenePaths.Length))
                        {
                            return false;
                        }

                        var dependencies = AssetDatabase.GetDependencies(scenePath, true);

                        foreach (var dependency in
                                 dependencies)
                        {
                            ScanAsset(dependency,
                                false); // All dependencies of this asset were already included recursively by Unity
                        }
                    }
                }

                return true;
            }
            finally
            {
                if (showProgressBar)
                {
                    EditorUtility.ClearProgressBar();
                }

                UnitySerializationUtility.ForceEditorModeSerialization = formerForceEditorModeSerialization;
            }
        }

        public bool ScanAsset(string assetPath, bool includeAssetDependencies)
        {
            if (includeAssetDependencies)
            {
                if (scannedPathsWithDependencies.Contains(assetPath))
                {
                    return true; // Already scanned this asset
                }

                scannedPathsWithDependencies.Add(assetPath);
                scannedPathsNoDependencies.Add(assetPath);
            }
            else
            {
                if (scannedPathsNoDependencies.Contains(assetPath))
                {
                    return true; // Already scanned this asset
                }

                scannedPathsNoDependencies.Add(assetPath);
            }

            if (assetPath.EndsWith(".unity"))
            {
                return ScanScenes(new[] { assetPath }, includeAssetDependencies, false);
            }

            if (!(assetPath.EndsWith(".asset") || assetPath.EndsWith(".prefab")))
                // ScanAsset can only scan .unity, .asset and .prefab assets.
            {
                return false;
            }

            var formerForceEditorModeSerialization = UnitySerializationUtility.ForceEditorModeSerialization;

            try
            {
                UnitySerializationUtility.ForceEditorModeSerialization = true;

                Object[] assets = AssetDatabase.LoadAllAssetsAtPath(assetPath);

                if (assets == null || assets.Length == 0)
                {
                    return false;
                }

                foreach (Object asset in assets)
                {
                    if (asset == null)
                    {
                        continue;
                    }

                    ScanObject(asset);
                }

                if (includeAssetDependencies)
                {
                    var dependencies = AssetDatabase.GetDependencies(assetPath, true);

                    foreach (var dependency in
                             dependencies)
                    {
                        ScanAsset(dependency, false); // All dependencies were already included recursively by Unity
                    }
                }

                return true;
            }
            finally
            {
                UnitySerializationUtility.ForceEditorModeSerialization = formerForceEditorModeSerialization;
            }
        }

        public void ScanObject(Object obj)
        {
            if (obj is ISerializationCallbackReceiver)
            {
                var formerForceEditorModeSerialization = UnitySerializationUtility.ForceEditorModeSerialization;

                try
                {
                    UnitySerializationUtility.ForceEditorModeSerialization = true;
                    allowRegisteringScannedTypes = true;
                    (obj as ISerializationCallbackReceiver).OnBeforeSerialize();
                }
                finally
                {
                    allowRegisteringScannedTypes = false;
                    UnitySerializationUtility.ForceEditorModeSerialization = formerForceEditorModeSerialization;
                }
            }
        }

        public List<Type> EndScan()
        {
            if (!scanning)
            {
                throw new InvalidOperationException("Cannot end a scan when scanning has not begun.");
            }

            var results = new HashSet<Type>();

            foreach (Type type in seenSerializedTypes)
            {
                GatherValidAOTSupportTypes(type, results);
            }

            Dispose();
            return results.ToList();
        }

        private void OnLocatedEmitType(Type type)
        {
            if (!allowRegisteringScannedTypes)
            {
                return;
            }

            seenSerializedTypes.Add(type);
        }

        private void OnSerializedType(Type type)
        {
            if (!allowRegisteringScannedTypes)
            {
                return;
            }

            seenSerializedTypes.Add(type);
        }

        private void OnLocatedFormatter(IFormatter formatter)
        {
            Type type = formatter.SerializedType;
            if (type == null || !allowRegisteringScannedTypes)
            {
                return;
            }

            seenSerializedTypes.Add(type);
        }

        public static bool AllowRegisterType(Type type)
        {
            if (IsEditorOnlyAssembly(type.Assembly))
            {
                return false;
            }

            if (type.IsGenericType)
            {
                foreach (Type parameter in type.GetGenericArguments())
                {
                    if (!AllowRegisterType(parameter))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private static bool IsEditorOnlyAssembly(Assembly assembly)
        {
            if (EditorAssemblyNames.Contains(assembly.GetName().Name))
            {
                return true;
            }

            bool result;
            if (!IsEditorOnlyAssembly_Cache.TryGetValue(assembly, out result))
            {
                try
                {
                    var name = assembly.GetName().Name;
                    var guids = AssetDatabase.FindAssets(name);
                    var paths = new string[guids.Length];

                    var dllCount = 0;
                    var dllIndex = 0;

                    for (var i = 0; i < guids.Length; i++)
                    {
                        paths[i] = AssetDatabase.GUIDToAssetPath(guids[i]);
                        if (paths[i].EndsWith(".dll", StringComparison.OrdinalIgnoreCase) ||
                            paths[i].EndsWith(".asmdef", StringComparison.OrdinalIgnoreCase))
                        {
                            dllCount++;
                            dllIndex = i;
                        }
                    }

                    if (dllCount == 1)
                    {
                        var path = paths[dllIndex];
                        var assetImporter = AssetImporter.GetAtPath(path);

                        if (assetImporter is PluginImporter)
                        {
                            var pluginImporter = assetImporter as PluginImporter;

                            if (!pluginImporter.GetCompatibleWithEditor())
                            {
                                result = false;
                            }
                            else if (pluginImporter.DefineConstraints.Any(n => n == "UNITY_EDITOR"))
                            {
                                result = true;
                            }
                            else
                            {
                                var isCompatibleWithAnyNonEditorPlatform = false;

                                foreach (FieldInfo member in typeof(BuildTarget).GetFields(BindingFlags.Public |
                                             BindingFlags.Static))
                                {
                                    var platform = (BuildTarget)member.GetValue(null);

                                    var asInt = Convert.ToInt32(platform);

                                    if (member.IsDefined(typeof(ObsoleteAttribute)) || asInt < 0)
                                    {
                                        continue;
                                    }

                                    if (pluginImporter.GetCompatibleWithPlatform(platform))
                                    {
                                        isCompatibleWithAnyNonEditorPlatform = true;
                                        break;
                                    }
                                }

                                result = !isCompatibleWithAnyNonEditorPlatform;
                            }
                        }
                        else if (assetImporter is AssemblyDefinitionImporter)
                        {
                            var asmDefImporter = assetImporter as AssemblyDefinitionImporter;
                            AssemblyDefinitionAsset asset =
                                AssetDatabase.LoadAssetAtPath<AssemblyDefinitionAsset>(path);

                            if (asset == null)
                            {
                                result = false;
                                goto HasResult;
                            }

                            AssemblyDefinitionData data = JsonUtility.FromJson<AssemblyDefinitionData>(asset.text);

                            if (data != null)
                            {
                                if (data.defineConstraints != null)
                                {
                                    for (var i = 0; i < data.defineConstraints.Length; i++)
                                    {
                                        if (data.defineConstraints[i].Trim() == "UNITY_EDITOR")
                                        {
                                            result = true;
                                            goto HasResult;
                                        }
                                    }
                                }
                            }

                            result = false;
                        }
                        else
                        {
                            result = false;
                        }
                    }
                    else
                    {
                        // There's either 0 or multiple of them; either way, we guess it will
                        // be included in the build, so we return false, it's probably not
                        // an editor only assembly.
                        result = false;
                    }

                    HasResult:

                    IsEditorOnlyAssembly_Cache.Add(assembly, result);
                }
                catch (Exception ex)
                {
                    Debug.LogException(ex);
                    IsEditorOnlyAssembly_Cache[assembly] = false;
                }
            }

            return result;
        }

        private static void GatherValidAOTSupportTypes(Type type, HashSet<Type> results)
        {
            if (type.IsGenericType && (type.IsGenericTypeDefinition || !type.IsFullyConstructedGenericType()))
            {
                return;
            }

            if (!AllowRegisterType(type))
            {
                return;
            }

            if (results.Add(type))
            {
                if (type.IsGenericType)
                {
                    foreach (Type arg in type.GetGenericArguments())
                    {
                        GatherValidAOTSupportTypes(arg, results);
                    }
                }
            }
        }

        private static bool DisplaySmartUpdatingCancellableProgressBar(string title, string details, float progress,
            int updateIntervalByMS = 200, int updateIntervalByCall = 50)
        {
            var updateProgressBar =
                smartProgressBarWatch.ElapsedMilliseconds >= updateIntervalByMS
                || ++smartProgressBarDisplaysSinceLastUpdate >= updateIntervalByCall;

            if (updateProgressBar)
            {
                smartProgressBarWatch.Stop();
                smartProgressBarWatch.Reset();
                smartProgressBarWatch.Start();

                smartProgressBarDisplaysSinceLastUpdate = 0;

                if (EditorUtility.DisplayCancelableProgressBar(title, details, progress))
                {
                    return true;
                }
            }

            return false;
        }

        [Serializable]
        private class VersionDefine
        {
            public string name;
            public string expression;
            public string define;
        }

        [Serializable]
        private class AssemblyDefinitionData
        {
            public string name;
            public string rootNamespace;
            public string[] references;
            public string[] includePlatforms;
            public string[] excludePlatforms;
            public bool allowUnsafeCode;
            public bool overrideReferences;
            public string[] precompiledReferences;
            public bool autoReferenced;
            public string[] defineConstraints;
            public VersionDefine[] versionDefines;
            public bool noEngineReferences;
        }
    }
}

#endif
