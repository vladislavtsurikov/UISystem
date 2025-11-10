#if UNITY_EDITOR
using UnityEngine;
using VladislavTsurikov.ComponentStack.Editor.Core;
using VladislavTsurikov.IMGUIUtility.Editor;
using VladislavTsurikov.IMGUIUtility.Editor.ElementStack;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.OverlapCheckSettings;

namespace VladislavTsurikov.MegaWorld.Editor.Common.Settings.OverlapCheckSettings
{
    [ElementEditor(typeof(OverlapCheckSettingsPreference))]
    public class OverlapCheckSettingsPreferenceEditor : IMGUIElementEditor
    {
        private OverlapCheckSettingsPreference Settings => (OverlapCheckSettingsPreference)Target;

        public override void OnGUI() =>
            Settings.MultiplyFindSize = CustomEditorGUILayout.FloatField(new GUIContent("Multiply Find Size",
                    "If you increase the Overlap Shape, but the object spawns inside another Overlap Shape, this means that the algorithm did not find this object, increase this parameter, but this will slow down the spawn."),
                Settings.MultiplyFindSize);
    }
}
#endif
