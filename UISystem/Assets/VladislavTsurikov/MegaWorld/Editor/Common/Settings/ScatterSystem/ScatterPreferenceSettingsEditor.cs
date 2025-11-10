#if UNITY_EDITOR
using UnityEngine;
using VladislavTsurikov.ComponentStack.Editor.Core;
using VladislavTsurikov.IMGUIUtility.Editor;
using VladislavTsurikov.IMGUIUtility.Editor.ElementStack;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.ScatterSystem;

namespace VladislavTsurikov.MegaWorld.Editor.Common.Settings.ScatterSystem
{
    [ElementEditor(typeof(ScatterPreferenceSettings))]
    public class ScatterPreferenceSettingsEditor : IMGUIElementEditor
    {
        private ScatterPreferenceSettings PreferenceSettings => (ScatterPreferenceSettings)Target;

        public override void OnGUI() =>
            PreferenceSettings.MaxChecks = Mathf.Max(1,
                CustomEditorGUILayout.IntField(new GUIContent("Max Checks"), PreferenceSettings.MaxChecks));
    }
}
#endif
