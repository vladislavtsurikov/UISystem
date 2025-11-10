#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace VladislavTsurikov.IMGUIUtility.Editor
{
    public abstract class BaseWindow<T> : EditorWindow where T : EditorWindow
    {
        private static T s_instance;

        protected double LastRepaintTime;

        private static T Window
        {
            get
            {
                T[] windows = Resources.FindObjectsOfTypeAll<T>();
                return windows.Length > 0 ? windows[0] : null;
            }
        }

        protected VisualElement Root => rootVisualElement;
        protected VisualElement windowLayout { get; set; }

        protected virtual bool UseCustomRepaintInterval => false;
        protected virtual double CustomRepaintIntervalDuringPlayMode => 0.4f;
        protected virtual double CustomRepaintIntervalWhileIdle => 0.6f;
        protected virtual bool RepaintOnInspectorUpdate => true;

        public static bool IsOpen { get; private set; }

        public static T Instance
        {
            get
            {
                if (s_instance != null)
                {
                    return s_instance;
                }

                s_instance = Window;
                if (s_instance != null)
                {
                    return s_instance;
                }

                s_instance = GetWindow<T>();
                return s_instance;
            }
        }

        protected virtual void Update()
        {
            if (!UseCustomRepaintInterval)
            {
                return;
            }

            if ((EditorApplication.isPlaying && EditorApplication.timeSinceStartup - LastRepaintTime <
                    CustomRepaintIntervalDuringPlayMode) ||
                (!EditorApplication.isPlayingOrWillChangePlaymode &&
                 EditorApplication.timeSinceStartup - LastRepaintTime < CustomRepaintIntervalWhileIdle))
            {
                Repaint();
            }
        }

        protected virtual void OnEnable() => IsOpen = true;

        protected virtual void OnDisable() => IsOpen = false;

        protected virtual void OnDestroy()
        {
        }

        protected virtual void OnGUI()
        {
            CustomEditorGUILayout.ScreenRect = position;

            EditorGUI.indentLevel = 0;

            CustomEditorGUILayout.IsInspector = false;

            Event current = Event.current;

            if (current.type == EventType.Repaint)
            {
                LastRepaintTime = EditorApplication.timeSinceStartup;
            }
        }

        /// <summary> Called 10 frames per second to give the inspector a chance to update </summary>
        protected virtual void OnInspectorUpdate()
        {
            if (RepaintOnInspectorUpdate)
            {
                Repaint();
            }
        }

        public static void OpenWindow(string windowTitle)
        {
            Instance.Show();
            Instance.titleContent.text = windowTitle;
        }
    }
}
#endif
