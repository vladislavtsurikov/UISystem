#if UNITY_EDITOR
using VladislavTsurikov.ComponentStack.Editor.Core;
using VladislavTsurikov.MegaWorld.Editor.Common;
using VladislavTsurikov.MegaWorld.Editor.Core.SelectionDatas;
using VladislavTsurikov.MegaWorld.Editor.Core.Window;
using VladislavTsurikov.MegaWorld.Editor.PrecisePlaceTool.PrototypeSettings;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.OverlapCheckSettings;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.TransformElementSystem;
using VladislavTsurikov.MegaWorld.Runtime.Core.GlobalSettings.ElementsSystem;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes;

namespace VladislavTsurikov.MegaWorld.Editor.PrecisePlaceTool.GUI
{
    [ElementEditor(typeof(PrecisePlaceTool))]
    [DrawersSelectionDatas(typeof(PrecisePlacePrototypesDrawer))]
    public class PrecisePlaceToolEditor : ToolWindowEditor
    {
        public override void DrawButtons() => UndoEditor.DrawButtons(TargetType, WindowData.Instance.SelectedData);

        protected override void DrawPrototypeSettings(Prototype proto)
        {
            var settings =
                (PrecisePlaceToolSettings)ToolsComponentStack.GetElement(typeof(PrecisePlaceTool),
                    typeof(PrecisePlaceToolSettings));

            proto.ComponentStackManager.DrawElement(typeof(PrecisePlaceSettings), typeof(PrecisePlaceTool));
            proto.ComponentStackManager.DrawElement(typeof(SuccessSettings));

            if (settings.OverlapCheck)
            {
                proto.ComponentStackManager.DrawElement(typeof(OverlapCheckSettings));
            }

            if (settings.UseTransformComponents)
            {
                proto.ComponentStackManager.DrawElement(typeof(TransformComponentSettings));
            }
        }
    }
}
#endif
