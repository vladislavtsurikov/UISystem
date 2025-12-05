#if UNITY_EDITOR
using VladislavTsurikov.ComponentStack.Editor.Core;
using VladislavTsurikov.MegaWorld.Editor.Common;
using VladislavTsurikov.MegaWorld.Editor.Core.Window;

namespace VladislavTsurikov.MegaWorld.Editor.SprayBrushTool.GUI
{
    [ElementEditor(typeof(SprayBrushTool))]
    public class SprayBrushToolEditor : ToolWindowEditor
    {
        public override void DrawButtons() => UndoEditor.DrawButtons(TargetType, WindowData.Instance.SelectedData);
    }
}
#endif
