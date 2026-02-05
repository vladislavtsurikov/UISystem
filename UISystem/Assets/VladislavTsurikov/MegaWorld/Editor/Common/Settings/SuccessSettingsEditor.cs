#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.Nody.Editor.Core;
using VladislavTsurikov.Core.Editor;
using VladislavTsurikov.IMGUIUtility.Editor;
using VladislavTsurikov.IMGUIUtility.Editor.ElementStack;
using VladislavTsurikov.IMGUIUtility.Editor.ElementStack.ReorderableList;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings;

namespace VladislavTsurikov.MegaWorld.Editor.Common.Settings
{
    [DontDrawFoldout]
    [ElementEditor(typeof(SuccessSettings))]
    public class SuccessSettingsEditor : IMGUIElementEditor
    {
        private readonly GUIContent _success = new("Success (%)");
        private SuccessSettings _successSettings => (SuccessSettings)Target;

        public override void OnGUI() =>
            _successSettings.SuccessValue =
                EditorGUILayout.Slider(_success, _successSettings.SuccessValue, 0f, 100f);
    }
}
#endif
