#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.IMGUIUtility.Editor;
using VladislavTsurikov.RendererStack.Runtime.Core.Preferences;

namespace VladislavTsurikov.RendererStack.Editor.Core.Preferences
{
    [CustomEditor(typeof(RendererStackSettings))]
    public class RendererStackSettingsEditor : UnityEditor.Editor
    {
        private RendererStackSettings _stackSettings;

        private void OnEnable() => _stackSettings = (RendererStackSettings)target;

        public override void OnInspectorGUI()
        {
            CustomEditorGUILayout.IsInspector = true;
            OnGUI(_stackSettings);
        }

        private static void OnGUI(RendererStackSettings stackSettings)
        {
            stackSettings.AutoShaderConversion = CustomEditorGUILayout.Toggle(new GUIContent("Auto Shader Conversion"),
                stackSettings.AutoShaderConversion);
            stackSettings.ShowRenderModelData = CustomEditorGUILayout.Toggle(new GUIContent("Show Render Model Data"),
                stackSettings.ShowRenderModelData);
            stackSettings.RenderDirectToCamera = CustomEditorGUILayout.Toggle(new GUIContent("Render Direct To Camera"),
                stackSettings.RenderDirectToCamera);
            stackSettings.RenderSceneCameraInPlayMode = CustomEditorGUILayout.Toggle(
                new GUIContent("Render Scene Camera in PlayMode"), stackSettings.RenderSceneCameraInPlayMode);

            stackSettings.RenderImposter =
                CustomEditorGUILayout.Toggle(new GUIContent("Render Imposter"), stackSettings.RenderImposter);
            stackSettings.ForceUpdateRendererData =
                CustomEditorGUILayout.Toggle(new GUIContent("Force Update Renderer Data"),
                    stackSettings.ForceUpdateRendererData);

            GUILayout.Space(5);
        }

        [SettingsProvider]
        public static SettingsProvider PreferencesGUI()
        {
            var provider = new SettingsProvider("Preferences/Renderer Stack", SettingsScope.User)
            {
                label = "Renderer Stack",
                guiHandler = _ => { OnGUI(RendererStackSettings.Instance); },
                keywords = new HashSet<string>(new[] { "Renderer Stack" })
            };

            return provider;
        }
    }
}
#endif
