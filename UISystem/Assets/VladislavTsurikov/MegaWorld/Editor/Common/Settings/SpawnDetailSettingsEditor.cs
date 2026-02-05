#if UNITY_EDITOR
using UnityEditor;
using System;
using UnityEngine;
using VladislavTsurikov.Nody.Editor.Core;
using VladislavTsurikov.Core.Editor;
using VladislavTsurikov.IMGUIUtility.Editor;
using VladislavTsurikov.IMGUIUtility.Editor.ElementStack;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings;

namespace VladislavTsurikov.MegaWorld.Editor.Common.Settings
{
    [Serializable]
    [ElementEditor(typeof(SpawnDetailSettings))]
    public class SpawnDetailSettingsEditor : IMGUIElementEditor
    {
        private readonly GUIContent _density = new("Density");

        private readonly GUIContent _failureRate = new("Failure Rate (%)",
            "The larger this value, the less likely it is to spawn an object.");

        private SpawnDetailSettings _spawnDetailSettings;
        private readonly GUIContent _useRandomOpacity = new("Use Random Opacity");

        public override void OnEnable() => _spawnDetailSettings = (SpawnDetailSettings)Target;

        public override void OnGUI()
        {
            _spawnDetailSettings.UseRandomOpacity =
                EditorGUILayout.Toggle(_useRandomOpacity, _spawnDetailSettings.UseRandomOpacity);
            _spawnDetailSettings.Density =
                EditorGUILayout.IntSlider(_density, _spawnDetailSettings.Density, 0, 10);
            _spawnDetailSettings.FailureRate =
                EditorGUILayout.Slider(_failureRate, _spawnDetailSettings.FailureRate, 0f, 100f);
        }
    }
}
#endif
