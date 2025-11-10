#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using VladislavTsurikov.IMGUIUtility.Editor;
using VladislavTsurikov.MegaWorld.Runtime.Core.PreferencesSystem;

namespace VladislavTsurikov.MegaWorld.Editor.Core.PreferencesSystem
{
    [CustomEditor(typeof(PreferencesSettings))]
    public class PreferencesSettingsEditor : UnityEditor.Editor
    {
        private PreferencesSettings _settings;

        private void OnEnable() => _settings = (PreferencesSettings)target;

        public override void OnInspectorGUI()
        {
            CustomEditorGUILayout.IsInspector = true;
            OnGUI(_settings);
        }

        private static void OnGUI(PreferencesSettings preferencesSettings)
        {
            EditorGUI.BeginChangeCheck();

            CustomEditorGUILayout.HelpBox(
                "These settings are for more advanced users. In most cases, you do not need to configure anything here.");

            preferencesSettings.StackEditor.OnGUI();

            if (EditorGUI.EndChangeCheck())
            {
                preferencesSettings.Save();
            }
        }

        [SettingsProvider]
        public static SettingsProvider PreferencesGUI()
        {
            var provider = new SettingsProvider("Preferences/Mega World", SettingsScope.User)
            {
                label = "Mega World",
                guiHandler = searchContext => { OnGUI(PreferencesSettings.Instance); },
                keywords = new HashSet<string>(new[] { "Mega World" })
            };

            return provider;
        }
    }
}
#endif
