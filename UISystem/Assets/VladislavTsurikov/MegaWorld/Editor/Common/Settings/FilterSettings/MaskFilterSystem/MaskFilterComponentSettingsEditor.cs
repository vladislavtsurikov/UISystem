#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.ComponentStack.Editor.Core;
using VladislavTsurikov.IMGUIUtility.Editor.ElementStack;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.FilterSettings.MaskFilterSystem;

namespace VladislavTsurikov.MegaWorld.Editor.Common.Settings.FilterSettings.MaskFilterSystem
{
    [ElementEditor(typeof(MaskFilterComponentSettings))]
    public class MaskFilterComponentSettingsEditor : IMGUIElementEditor
    {
        public static bool ChangedGUI;
        private MaskFilterComponentSettings _maskFilterComponentSettings;
        private MaskFilterStackEditor _maskFilterStackEditor;

        public override void OnEnable()
        {
            _maskFilterComponentSettings = (MaskFilterComponentSettings)Target;
            _maskFilterStackEditor = new MaskFilterStackEditor(new GUIContent("Mask Filters Settings"),
                _maskFilterComponentSettings.MaskFilterStack);
        }

        public override void OnGUI()
        {
            EditorGUI.BeginChangeCheck();

            _maskFilterStackEditor.OnGUI();

            if (EditorGUI.EndChangeCheck())
            {
                ChangedGUI = true;
            }
        }
    }
}
#endif
