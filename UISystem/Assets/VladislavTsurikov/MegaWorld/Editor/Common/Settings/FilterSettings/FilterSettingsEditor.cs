#if UNITY_EDITOR
using UnityEngine;
using VladislavTsurikov.ComponentStack.Editor.Core;
using VladislavTsurikov.IMGUIUtility.Editor;
using VladislavTsurikov.IMGUIUtility.Editor.ElementStack;
using VladislavTsurikov.MegaWorld.Editor.Common.Settings.FilterSettings.MaskFilterSystem;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.FilterSettings;

namespace VladislavTsurikov.MegaWorld.Editor.Common.Settings.FilterSettings
{
    [ElementEditor(typeof(Runtime.Common.Settings.FilterSettings.FilterSettings))]
    public class FilterSettingsEditor : IMGUIElementEditor
    {
        private Runtime.Common.Settings.FilterSettings.FilterSettings _filterSettings;
        private MaskFilterComponentSettingsEditor _maskFilterComponentSettingsEditor;

        private SimpleFilterEditor _simpleFilterEditor;

        public override void OnEnable()
        {
            _filterSettings = (Runtime.Common.Settings.FilterSettings.FilterSettings)Target;

            _simpleFilterEditor = new SimpleFilterEditor();
            _simpleFilterEditor.Init(_filterSettings.SimpleFilter);
            _maskFilterComponentSettingsEditor = new MaskFilterComponentSettingsEditor();
            _maskFilterComponentSettingsEditor.Init(_filterSettings.MaskFilterComponentSettings);
        }

        public override void OnGUI()
        {
            _filterSettings.FilterType =
                (FilterType)CustomEditorGUILayout.EnumPopup(new GUIContent("Filter Type"), _filterSettings.FilterType);

            switch (_filterSettings.FilterType)
            {
                case FilterType.SimpleFilter:
                {
                    _simpleFilterEditor.OnGUI();
                    break;
                }
                case FilterType.MaskFilter:
                {
                    CustomEditorGUILayout.HelpBox("\"Mask Filter\" works only with Unity terrain");
                    _maskFilterComponentSettingsEditor.OnGUI();
                    break;
                }
            }
        }
    }
}
#endif
