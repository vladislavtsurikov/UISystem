#if UNITY_EDITOR
using System;
using UnityEngine;
using VladislavTsurikov.ComponentStack.Editor.Core;
using VladislavTsurikov.IMGUIUtility.Editor;
using VladislavTsurikov.IMGUIUtility.Editor.ElementStack;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.BrushSettings;
using VladislavTsurikov.MegaWorld.Runtime.Core.PreferencesSystem;

namespace VladislavTsurikov.MegaWorld.Editor.SprayBrushTool.GUI
{
    [ElementEditor(typeof(BrushSettings))]
    public class BrushSettingsEditor : IMGUIElementEditor
    {
        [NonSerialized]
        private readonly GUIContent _brushSize = new("Brush Size",
            "Selected prototypes will only spawn in this range around the center of Brush.");

        [NonSerialized]
        private readonly GUIContent _spacing = new("Spacing", "Controls the distance between brush marks.");

        private BrushSettings _settings;

        public override void OnEnable() => _settings = (BrushSettings)Target;

        public override void OnGUI()
        {
            _settings.Spacing = CustomEditorGUILayout.Slider(_spacing, _settings.Spacing, 0.1f, 5);

            _settings.BrushSize = CustomEditorGUILayout.Slider(_brushSize, _settings.BrushSize, 0.1f,
                PreferenceElementSingleton<BrushPreferenceSettings>.Instance.MaxBrushSize);
        }
    }
}
#endif
