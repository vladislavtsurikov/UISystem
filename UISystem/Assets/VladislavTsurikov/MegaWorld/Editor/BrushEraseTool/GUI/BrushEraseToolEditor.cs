#if UNITY_EDITOR
using VladislavTsurikov.Nody.Editor.Core;
using VladislavTsurikov.Core.Editor;
using VladislavTsurikov.MegaWorld.Editor.Common;
using VladislavTsurikov.MegaWorld.Editor.Core.Window;

namespace VladislavTsurikov.MegaWorld.Editor.BrushEraseTool
{
    [ElementEditor(typeof(BrushEraseTool))]
    public class BrushEraseToolEditor : ToolWindowEditor
    {
        public override void DrawButtons() => UndoEditor.DrawButtons(TargetType, WindowData.Instance.SelectedData);
    }
}
#endif
