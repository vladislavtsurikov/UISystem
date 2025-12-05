#if UNITY_EDITOR
using System;
using System.Linq;
using UnityEngine;
using VladislavTsurikov.AttributeUtility.Runtime;
using VladislavTsurikov.IMGUIUtility.Editor;
using VladislavTsurikov.MegaWorld.Editor.Core.Window;
using VladislavTsurikov.MegaWorld.Runtime.Core.MonoBehaviour;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.PrototypeGameObject;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.PrototypeTerrainObject;

namespace VladislavTsurikov.MegaWorld.Editor.Common
{
    public static class UndoEditor
    {
        public static void DrawButtons(Type toolType, SelectedData selectedData)
        {
            if (!typeof(ToolWindow).IsAssignableFrom(toolType) &&
                !typeof(MonoBehaviourTool).IsAssignableFrom(toolType))
            {
                return;
            }

            if (!selectedData.HasOneSelectedGroup())
            {
                return;
            }

            if (selectedData.SelectedGroup.PrototypeType != typeof(PrototypeGameObject)
                && selectedData.SelectedGroup.PrototypeType != typeof(PrototypeTerrainObject))
            {
                return;
            }

            if (toolType.GetAttribute<SupportedPrototypeTypesAttribute>().PrototypeTypes
                .Contains(selectedData.SelectedGroup.PrototypeType))
            {
                OnGUI();
            }
        }

        private static void OnGUI()
        {
            GUILayout.BeginHorizontal();
            {
                GUILayout.Space(CustomEditorGUILayout.GetCurrentSpace());
                if (CustomEditorGUILayout.ClickButton("Undo (" + Undo.Editor.Undo.UndoRecordCount + "/" +
                                                      Undo.Editor.Undo.MaxNumberOfUndo + ")"))
                {
                    Undo.Editor.Undo.PerformUndo();
                }

                GUILayout.Space(3);
                if (CustomEditorGUILayout.ClickButton("Undo All"))
                {
                    Undo.Editor.Undo.PerformUndoAll();
                }

                GUILayout.Space(5);
            }
            GUILayout.EndHorizontal();
        }
    }
}
#endif
