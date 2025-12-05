#if UNITY_EDITOR
using UnityEngine;
using VladislavTsurikov.ComponentStack.Editor.Core;
using VladislavTsurikov.IMGUIUtility.Editor;
using VladislavTsurikov.IMGUIUtility.Editor.ElementStack;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.BrushSettings;

namespace VladislavTsurikov.MegaWorld.Editor.Common.Settings.BrushSettings
{
    [ElementEditor(typeof(BrushPreferenceSettings))]
    public class BrushPreferenceSettingsEditor : IMGUIElementEditor
    {
        private BrushPreferenceSettings PreferenceSettings => (BrushPreferenceSettings)Target;

        public override void OnGUI() =>
            PreferenceSettings.MaxBrushSize = Mathf.Max(0.5f,
                CustomEditorGUILayout.FloatField(new GUIContent("Max Brush Size"), PreferenceSettings.MaxBrushSize));
    }
}
#endif
