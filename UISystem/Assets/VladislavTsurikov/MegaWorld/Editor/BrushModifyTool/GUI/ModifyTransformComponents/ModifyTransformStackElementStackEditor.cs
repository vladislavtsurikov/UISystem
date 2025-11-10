#if UNITY_EDITOR
using UnityEngine;
using VladislavTsurikov.ComponentStack.Editor.Core;
using VladislavTsurikov.IMGUIUtility.Editor.ElementStack;
using VladislavTsurikov.MegaWorld.Editor.BrushModifyTool.ModifyTransformComponents;

namespace VladislavTsurikov.MegaWorld.Editor.BrushModifyTool.GUI.ModifyTransformComponents
{
    [ElementEditor(typeof(ModifyTransformSettings))]
    public class ModifyTransformStackElementStackEditor : IMGUIElementEditor
    {
        private ModifyTransformStackEditor _modifyTransformStackEditor;
        private ModifyTransformSettings _settings;

        public override void OnEnable()
        {
            _settings = (ModifyTransformSettings)Target;
            _modifyTransformStackEditor = new ModifyTransformStackEditor(new GUIContent("Modify Transform Components"),
                _settings.ModifyTransformComponentStack);
        }

        public override void OnGUI() => _modifyTransformStackEditor.OnGUI();
    }
}
#endif
