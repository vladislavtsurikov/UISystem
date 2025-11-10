using System.Collections.Generic;
using UnityEngine;
using VladislavTsurikov.ScriptableObjectUtility.Editor;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace VladislavTsurikov.ScriptableObjectUtility.Runtime
{
    public class ScriptableObjectRegistry<T> : ScriptableObject
#if UNITY_EDITOR
        , IOnEditorDestroy
#endif
        where T : ScriptableObject
    {
#if UNITY_EDITOR
        private static bool _findAllScriptableObject = true;
#endif

        private static readonly List<T> _allInstances = new();


        private static readonly HashSet<int> _addedInstanceIDs = new();

        public static List<T> AllInstances
        {
            get
            {
#if UNITY_EDITOR
                if (_findAllScriptableObject)
                {
                    FindAllScriptableObject();
                }
#endif

                return _allInstances;
            }
        }

        private void OnEnable()
        {
#if UNITY_EDITOR
            if (_findAllScriptableObject)
            {
                FindAllScriptableObject();
            }
#endif

            AddInstance(this as T);
        }

        private void OnDisable()
        {
            AllInstances.Remove(this as T);

            var instanceID = GetInstanceID();
            if (_addedInstanceIDs.Contains(instanceID))
            {
                _addedInstanceIDs.Remove(instanceID);
            }
        }

        private static void AddInstance(T instance)
        {
            if (instance == null)
            {
                return;
            }

            var instanceID = instance.GetInstanceID();
            if (!_addedInstanceIDs.Contains(instanceID))
            {
                _addedInstanceIDs.Add(instanceID);
                _allInstances.Add(instance);
            }
        }

#if UNITY_EDITOR
        void IOnEditorDestroy.OnEditorDestroy() => OnDisable();

        private static void FindAllScriptableObject()
        {
            _findAllScriptableObject = false;

            var guids = AssetDatabase.FindAssets($"t:{typeof(T).Name}");

            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                T instance = AssetDatabase.LoadAssetAtPath<T>(path);
                AddInstance(instance);
            }
        }
#endif
    }
}
