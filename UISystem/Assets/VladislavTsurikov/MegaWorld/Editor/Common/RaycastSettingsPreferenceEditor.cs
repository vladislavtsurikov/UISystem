#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.Nody.Editor.Core;
using VladislavTsurikov.Core.Editor;
using VladislavTsurikov.IMGUIUtility.Editor;
using VladislavTsurikov.IMGUIUtility.Editor.ElementStack;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings;

namespace VladislavTsurikov.MegaWorld.Editor.Common
{
    [ElementEditor(typeof(RaycastPreferenceSettings))]
    public class RaycastSettingsPreferenceEditor : IMGUIElementEditor
    {
        private RaycastPreferenceSettings Settings => (RaycastPreferenceSettings)Target;

        public override void OnGUI()
        {
            Settings.RaycastType =
                (RaycastType)EditorGUILayout.EnumPopup(new GUIContent("Raycast Type", ""), Settings.RaycastType);
            Settings.Offset = CustomEditorGUILayout.FloatField(new GUIContent("Offset",
                    "If you want to spawn objects under pawns or inside buildings or in other similar cases. You need to decrease the Spawn Check Offset."),
                Settings.Offset);
        }
    }
}
#endif
