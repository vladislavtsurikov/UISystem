#if UNITY_EDITOR
using System;
using UnityEditor;
using VladislavTsurikov.IMGUIUtility.Editor;
using VladislavTsurikov.IMGUIUtility.Editor.ElementStack.IconStack;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas;

namespace VladislavTsurikov.MegaWorld.Editor.Core.SelectionDatas.Group.Prototypes
{
    public class IconPrototypesDrawer : PrototypesDrawer
    {
        private readonly IconStackEditor _iconStackEditor = new(true);
        private bool _selectPrototypeFoldout = true;

        public IconPrototypesDrawer(SelectionData selectionData, Type toolType) : base(selectionData, toolType)
        {
        }

        public override void OnGUI()
        {
            EditorGUILayout.BeginHorizontal();

            if (Data.SelectedData.SelectedGroup != null)
            {
                _selectPrototypeFoldout = CustomEditorGUILayout.Foldout(_selectPrototypeFoldout,
                    "Prototypes " + "(" + Data.SelectedData.SelectedGroup.GetPrototypeTypeName() + ")");
            }
            else
            {
                _selectPrototypeFoldout = CustomEditorGUILayout.Foldout(_selectPrototypeFoldout, "Prototypes ");
            }

            EditorGUILayout.EndHorizontal();

            if (_selectPrototypeFoldout)
            {
                ++EditorGUI.indentLevel;

                if (Data.SelectedData.SelectedGroup == null)
                {
                    _iconStackEditor.OnGUI("To Draw Prototype, you need to select one group");
                }
                else
                {
                    _iconStackEditor.AddIconCallback = Data.SelectedData.SelectedGroup.AddMissingPrototype;
                    _iconStackEditor.IconMenuCallback = PrototypeMenu;
                    _iconStackEditor.OnGUI(Data.SelectedData.SelectedGroup.PrototypeList,
                        Data.SelectedData.SelectedGroup.PrototypeType);
                }

                --EditorGUI.indentLevel;
            }
        }
    }
}
#endif
