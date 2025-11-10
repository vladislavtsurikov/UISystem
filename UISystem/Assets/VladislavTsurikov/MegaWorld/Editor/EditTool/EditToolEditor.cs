#if UNITY_EDITOR
using VladislavTsurikov.ComponentStack.Editor.Core;
using VladislavTsurikov.IMGUIUtility.Editor.ElementStack.ReorderableList;
using VladislavTsurikov.MegaWorld.Editor.Common;
using VladislavTsurikov.MegaWorld.Editor.Core.Window;
using VladislavTsurikov.MegaWorld.Editor.Core.Window.ElementSystem;

namespace VladislavTsurikov.MegaWorld.Editor.EditTool
{
    [ElementEditor(typeof(EditTool))]
    [DontDrawFoldout]
    public class EditToolEditor : ToolWindowEditor
    {
        public override void DrawButtons()
        {
            UndoEditor.DrawButtons(TargetType, WindowData.Instance.SelectedData);

            var editor =
                (EditToolSettingsEditor)ToolsComponentStackEditor.GetEditor(typeof(EditTool), typeof(EditToolSettings));
            editor.ActionStackEditor.DrawButtons();
        }

        protected override void DrawCommonSettings()
        {
        }
    }
}
#endif
