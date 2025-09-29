#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.CustomInspector.Runtime;

namespace VladislavTsurikov.CustomInspector.Editor
{
    public static class CustomInspectorPreferencesProvider
    {
        [SettingsProvider]
        public static SettingsProvider SettingsGUI()
        {
            var provider = new SettingsProvider("Preferences/CustomInspector", SettingsScope.User)
            {
                label = "Custom Inspector",
                guiHandler = searchContext =>
                {
                    CustomInspectorPreferences settings = CustomInspectorPreferences.Instance;

                    if (settings == null)
                    {
                        EditorGUILayout.HelpBox("CustomInspectorPreferences asset is missing.", MessageType.Warning);
                        return;
                    }

                    settings.ShowFieldWithHideInInspectorAttribute = EditorGUILayout.Toggle(
                        "Show Field With Hide In Inspector Attribute", settings.ShowFieldWithHideInInspectorAttribute);

                    if (GUI.changed)
                    {
                        EditorUtility.SetDirty(settings);
                    }
                },
                keywords = new HashSet<string>(new[] { "CustomInspector" })
            };

            return provider;
        }
    }
}
#endif
