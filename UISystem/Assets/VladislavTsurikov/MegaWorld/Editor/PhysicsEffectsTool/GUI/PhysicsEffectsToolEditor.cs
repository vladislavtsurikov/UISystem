#if UNITY_EDITOR
using VladislavTsurikov.ComponentStack.Editor.Core;
using VladislavTsurikov.MegaWorld.Editor.Common;
using VladislavTsurikov.MegaWorld.Editor.Common.PhysXPainter.Settings;
using VladislavTsurikov.MegaWorld.Editor.Core.Window;
using VladislavTsurikov.MegaWorld.Editor.Core.Window.ElementSystem;
using VladislavTsurikov.PhysicsSimulator.Runtime;

namespace VladislavTsurikov.MegaWorld.Editor.PhysicsEffectsTool.GUI
{
    [ElementEditor(typeof(PhysicsEffectsTool))]
    public class PhysicsEffectsToolEditor : ToolWindowEditor
    {
        public override void DrawButtons()
        {
            UndoEditor.DrawButtons(TargetType, WindowData.Instance.SelectedData);

            var editor =
                (PhysicsEffectsToolSettingsEditor)ToolsComponentStackEditor.GetEditor(typeof(PhysicsEffectsTool),
                    typeof(PhysicsEffectsToolSettings));
            editor.PhysicsEffectStackEditor.DrawButtons();
        }

        public override void DrawFirstSettings() =>
            PhysicsSimulatorSettingsEditor.OnGUI<GlobalTimeDisablePhysicsMode>(PhysicsSimulatorSettings.Instance,
                false);
    }
}
#endif
