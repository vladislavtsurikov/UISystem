#if UNITY_EDITOR
using VladislavTsurikov.ComponentStack.Editor.Core;
using VladislavTsurikov.MegaWorld.Editor.Common;
using VladislavTsurikov.MegaWorld.Editor.Common.PhysXPainter.Settings;
using VladislavTsurikov.MegaWorld.Editor.Core.Window;
using VladislavTsurikov.PhysicsSimulator.Runtime;

namespace VladislavTsurikov.MegaWorld.Editor.BrushPhysicsTool.GUI
{
    [ElementEditor(typeof(BrushPhysicsTool))]
    public class BrushPhysicsToolEditor : ToolWindowEditor
    {
        public override void DrawButtons() => UndoEditor.DrawButtons(TargetType, WindowData.Instance.SelectedData);

        public override void DrawFirstSettings() =>
            PhysicsSimulatorSettingsEditor.OnGUI<ObjectTimeDisablePhysicsMode>(PhysicsSimulatorSettings.Instance);
    }
}
#endif
