#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.IMGUIUtility.Editor;
using VladislavTsurikov.MegaWorld.Runtime.Core.GlobalSettings;

namespace VladislavTsurikov.MegaWorld.Editor.Core.Window
{
    public partial class MegaWorldWindow : BaseWindow<MegaWorldWindow>
    {
        protected override void OnEnable()
        {
            base.OnEnable();

            hideFlags = HideFlags.HideAndDontSave;

            SceneView.duringSceneGui += OnSceneGUI;
            EditorApplication.modifierKeysChanged += Repaint;
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            SceneView.duringSceneGui -= OnSceneGUI;
            EditorApplication.modifierKeysChanged -= Repaint;

            WindowData.Instance.WindowToolStack.OnDisable();

            WindowData.Instance.Save();
            GlobalSettings.Instance.Save();
        }

        private void OnSceneGUI(SceneView sceneView)
        {
            WindowData.Instance.SelectionData.DeleteNullElements();
            WindowData.Instance.SelectedData.SetSelectedData();
            UpdateSceneViewEvent();

            WindowData.Instance.WindowToolStack.DoSelectedTool();
        }
    }
}
#endif
