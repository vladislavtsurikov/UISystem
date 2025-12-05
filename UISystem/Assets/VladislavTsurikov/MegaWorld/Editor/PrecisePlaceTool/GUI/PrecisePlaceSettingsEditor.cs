#if UNITY_EDITOR
using UnityEngine;
using VladislavTsurikov.ComponentStack.Editor.Core;
using VladislavTsurikov.IMGUIUtility.Editor;
using VladislavTsurikov.IMGUIUtility.Editor.ElementStack;
using VladislavTsurikov.IMGUIUtility.Editor.ElementStack.ReorderableList;
using VladislavTsurikov.MegaWorld.Editor.PrecisePlaceTool.PrototypeSettings;

namespace VladislavTsurikov.MegaWorld.Editor.PrecisePlaceTool.GUI
{
    [DontDrawFoldout]
    [ElementEditor(typeof(PrecisePlaceSettings))]
    public class PrecisePlaceSettingsEditor : IMGUIElementEditor
    {
        private PrecisePlaceSettings _precisePlaceSettings;

        public override void OnEnable() => _precisePlaceSettings = (PrecisePlaceSettings)Target;

        public override void OnGUI() =>
            _precisePlaceSettings.PositionOffset = CustomEditorGUILayout.FloatField(new GUIContent("Position Offset"),
                _precisePlaceSettings.PositionOffset);
    }
}
#endif
