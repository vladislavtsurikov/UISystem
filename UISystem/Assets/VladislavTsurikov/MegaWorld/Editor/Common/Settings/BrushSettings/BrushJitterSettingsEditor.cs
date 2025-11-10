#if UNITY_EDITOR
using System;
using UnityEngine;
using VladislavTsurikov.IMGUIUtility.Editor;
using VladislavTsurikov.MegaWorld.Editor.Core.Window;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.BrushSettings;
using VladislavTsurikov.MegaWorld.Runtime.Core.PreferencesSystem;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.PrototypeTerrainDetail;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.PrototypeTerrainTexture;

namespace VladislavTsurikov.MegaWorld.Editor.Common.Settings.BrushSettings
{
    [Serializable]
    public class BrushJitterSettingsEditor
    {
        [NonSerialized]
        private readonly GUIContent _brushJitter = new("Jitter", "Control brush stroke randomness.");

        [NonSerialized]
        private readonly GUIContent _brushRotation = new("Brush Rotation", "Rotation of the brush.");

        [NonSerialized]
        private readonly GUIContent _brushScatter = new("Brush Scatter", "Randomize brush position by an offset.");

        [NonSerialized]
        private readonly GUIContent _brushSize = new("Brush Size",
            "Selected prototypes will only spawn in this range around the center of Brush.");

        public void OnGUI(Runtime.Common.Settings.BrushSettings.BrushSettings brush, BrushJitterSettings jitter)
        {
            brush.BrushSize = CustomEditorGUILayout.Slider(_brushSize, brush.BrushSize, 0.1f,
                PreferenceElementSingleton<BrushPreferenceSettings>.Instance.MaxBrushSize);

            jitter.BrushSizeJitter = CustomEditorGUILayout.Slider(_brushJitter, jitter.BrushSizeJitter, 0f, 1f);

            //CustomEditorGUILayout.Separator();

            jitter.BrushScatter = CustomEditorGUILayout.Slider(_brushScatter, jitter.BrushScatter, 0f, 1f);
            jitter.BrushScatterJitter = CustomEditorGUILayout.Slider(_brushJitter, jitter.BrushScatterJitter, 0f, 1f);

            //CustomEditorGUILayout.Separator();

            if (WindowData.Instance.SelectedData.HasOneSelectedGroup())
            {
                if (WindowData.Instance.SelectedData.SelectedGroup.PrototypeType == typeof(PrototypeTerrainDetail) ||
                    WindowData.Instance.SelectedData.SelectedGroup.PrototypeType == typeof(PrototypeTerrainTexture))
                {
                    brush.BrushRotation =
                        CustomEditorGUILayout.Slider(_brushRotation, brush.BrushRotation, -180f, 180f);
                    jitter.BrushRotationJitter =
                        CustomEditorGUILayout.Slider(_brushJitter, jitter.BrushRotationJitter, 0f, 1f);
                    //CustomEditorGUILayout.Separator();
                }
            }
        }
    }
}
#endif
