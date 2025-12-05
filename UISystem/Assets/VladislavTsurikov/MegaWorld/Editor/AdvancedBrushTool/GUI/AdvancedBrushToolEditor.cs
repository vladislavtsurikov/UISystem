#if UNITY_EDITOR
using VladislavTsurikov.ComponentStack.Editor.Core;
using VladislavTsurikov.MegaWorld.Editor.Common;
using VladislavTsurikov.MegaWorld.Editor.Core.Window;

namespace VladislavTsurikov.MegaWorld.Editor.AdvancedBrushTool
{
    [ElementEditor(typeof(AdvancedBrushTool))]
    public class AdvancedBrushToolEditor : ToolWindowEditor
    {
        public override void DrawButtons() => UndoEditor.DrawButtons(TargetType, WindowData.Instance.SelectedData);
    }
}
#endif
