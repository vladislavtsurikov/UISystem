#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.IMGUIUtility.Editor;
using VladislavTsurikov.RendererStack.Runtime.Core;

namespace VladislavTsurikov.RendererStack.Editor.Core.Windows
{
    public partial class RendererStackWindow : BaseWindow<RendererStackWindow>
    {
        private const string WindowTitle = "Renderer Stack";
        public const string KWindowMenuPath = "Window/Vladislav Tsurikov/Renderer Stack";

        protected override void OnEnable()
        {
            base.OnEnable();

            SceneView.duringSceneGui += OnSceneGUI;
            EditorApplication.modifierKeysChanged += Repaint;
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            SceneView.duringSceneGui -= OnSceneGUI;
            EditorApplication.modifierKeysChanged -= Repaint;
        }

        [MenuItem(KWindowMenuPath, false, 0)]
        public static void Open() => OpenWindow(WindowTitle);

        private void OnSceneGUI(SceneView sceneView)
        {
            if (Event.current.type == EventType.Repaint)
            {
                RendererStackManager.Instance?.DrawDebug();
            }
        }
    }
}
#endif
