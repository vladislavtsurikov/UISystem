using UnityEditor;
using UnityEngine;
using VladislavTsurikov.Math.Runtime;
using VladislavTsurikov.MegaWorld.Editor.Core.Window;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings;
using VladislavTsurikov.MegaWorld.Runtime.Core.GlobalSettings.ElementsSystem;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Attributes;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.PrototypeGameObject;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.PrototypeTerrainObject;
using VladislavTsurikov.ReflectionUtility;
using GUIUtility = UnityEngine.GUIUtility;

namespace VladislavTsurikov.MegaWorld.Editor.EditTool
{
#if UNITY_EDITOR
    [Name("Happy Artist/Edit")]
    [SupportMultipleSelectedGroups]
    [SupportedPrototypeTypes(new[] { typeof(PrototypeTerrainObject), typeof(PrototypeGameObject) })]
    [AddGlobalCommonComponents(new[] { typeof(TransformSpaceSettings), typeof(LayerSettings) })]
    [AddToolComponents(new[] { typeof(EditToolSettings) })]
    public class EditTool : ToolWindow
    {
        internal static CommonInstance FindObject;
        private bool _mouseUp = true;

        protected override void OnEnable() => _mouseUp = true;

        protected override void DoTool()
        {
            var editToolSettings =
                (EditToolSettings)ToolsComponentStack.GetElement(typeof(EditTool), typeof(EditToolSettings));

            var controlID = GUIUtility.GetControlID(EditorHash, FocusType.Passive);

            RemoveFindObjectsIfNecessary();

            if (FindObject == null)
            {
                editToolSettings.ActionStack.SelectedElement?.HandleButtons();
            }
            else
            {
                editToolSettings.ActionStack.SelectedElement?.OnMouseMove();
            }

            if (Event.current.type == EventType.Layout)
            {
                HandleUtility.AddDefaultControl(controlID);
            }
        }

        protected override void HandleKeyboardEvents()
        {
            var editToolSettings =
                (EditToolSettings)ToolsComponentStack.GetElement(typeof(EditTool), typeof(EditToolSettings));

            Tools.current = Tool.None;

            Event e = Event.current;

            switch (e.type)
            {
                case EventType.MouseDown:
                {
                    _mouseUp = false;
                    break;
                }
                case EventType.MouseUp:
                {
                    _mouseUp = true;
                    FindObject = null;
                    break;
                }
            }

            if (!_mouseUp)
            {
                return;
            }

            switch (e.keyCode)
            {
                case KeyCode.Space:
                {
                    if (EventType.KeyUp == e.type)
                    {
                        GlobalCommonComponentSingleton<TransformSpaceSettings>.Instance.TransformSpace =
                            GlobalCommonComponentSingleton<TransformSpaceSettings>.Instance.TransformSpace ==
                            TransformSpace.Global
                                ? TransformSpace.Local
                                : TransformSpace.Global;
                    }

                    break;
                }
            }

            editToolSettings.ActionStack.CheckShortcutCombos();
        }

        public override bool DisableToolIfUnityToolActive() => false;

        private static void RemoveFindObjectsIfNecessary()
        {
            if (FindObject == null)
            {
                return;
            }

            if (FindObject.IsValid() == false)
            {
                FindObject = null;
            }
        }
    }
#endif
}
