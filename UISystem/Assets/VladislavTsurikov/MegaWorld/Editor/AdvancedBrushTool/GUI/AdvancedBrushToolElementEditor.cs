#if UNITY_EDITOR
using UnityEngine;
using VladislavTsurikov.ComponentStack.Editor.Core;
using VladislavTsurikov.IMGUIUtility.Editor;
using VladislavTsurikov.IMGUIUtility.Editor.ElementStack;
using VladislavTsurikov.IMGUIUtility.Editor.ElementStack.ReorderableList;
using VladislavTsurikov.MegaWorld.Editor.Core.Window;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.PrototypeTerrainTexture;

namespace VladislavTsurikov.MegaWorld.Editor.AdvancedBrushTool
{
    [DontDrawFoldout]
    [ElementEditor(typeof(AdvancedBrushToolSettings))]
    public class AdvancedBrushToolElementEditor : IMGUIElementEditor
    {
        private AdvancedBrushToolSettings _advancedBrushToolSettings;

        public override void OnEnable() => _advancedBrushToolSettings = (AdvancedBrushToolSettings)Target;

        public override void OnGUI()
        {
            if (WindowData.Instance.SelectedData.HasOneSelectedGroup())
            {
                if (WindowData.Instance.SelectedData.SelectedGroup.PrototypeType == typeof(PrototypeTerrainTexture))
                {
                    _advancedBrushToolSettings.TextureTargetStrength = CustomEditorGUILayout.Slider(
                        new GUIContent("Target Strength"), _advancedBrushToolSettings.TextureTargetStrength, 0, 1);
                }
            }
        }
    }
}
#endif
