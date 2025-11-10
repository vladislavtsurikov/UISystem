using UnityEditor;
using UnityEngine;

namespace VladislavTsurikov.UnityUtility.Runtime
{
    public static class EditorAndRuntimeUpdate
    {
        public delegate void UpdateEvent();

        private static UpdateEvent _update;

        static EditorAndRuntimeUpdate()
        {
            if (Application.isPlaying)
            {
                UniversalToolkitRuntimeUpdate universalToolkitRuntimeUpdate = UniversalToolkitRuntimeUpdate.Instance;
            }
            else
            {
#if UNITY_EDITOR
                EditorApplication.update += EditorUpdate;
#endif
            }
        }

        public static void AddUpdateFunction(UpdateEvent function)
        {
            _update -= function;
            _update += function;
        }

        public static void RemoveUpdateFunction(UpdateEvent function) => _update -= function;

#if UNITY_EDITOR
        private static void EditorUpdate() => _update?.Invoke();
#endif

        [MonoBehaviourName("Universal Toolkit Runtime Update")]
        [DontDestroyOnLoad]
        public class UniversalToolkitRuntimeUpdate : MonoBehaviourSingleton<UniversalToolkitRuntimeUpdate>
        {
            private void Update() => _update?.Invoke();
        }
    }
}
