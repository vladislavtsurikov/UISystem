#if UNITY_EDITOR
using System;
using OdinSerializer.Utilities;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.ColorUtility.Runtime;
using VladislavTsurikov.IMGUIUtility.Editor;
using VladislavTsurikov.IMGUIUtility.Editor.ElementStack.IconStack;
using VladislavTsurikov.IMGUIUtility.Runtime.ElementStack.IconStack;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas;
using Object = UnityEngine.Object;

namespace VladislavTsurikov.MegaWorld.Editor.Core.SelectionDatas.Group
{
    public class IconGroupsDrawer : GroupsDrawer
    {
        private readonly IconStackEditor _iconStackEditor = new(true);
        private bool _selectPrototypeFoldout = true;

        public IconGroupsDrawer(SelectionData data, Type toolType) : base(data, toolType)
        {
            _iconStackEditor.AddIconCallback = AddGroup;
            _iconStackEditor.DrawIconRect = DrawIcon;
            _iconStackEditor.DrawWindowMenu = GroupWindowMenu;
            _iconStackEditor.IconMenuCallback = GroupMenu;
            _iconStackEditor.IconColor = SetIconColor;
            _iconStackEditor.IconSelected = SetSelectedAllPrototypes;
            _iconStackEditor.ZeroIconsWarning = "Drag & Drop Group Here";
            _iconStackEditor.DraggedIconType = true;
        }

        public override void OnGUI()
        {
            _selectPrototypeFoldout = CustomEditorGUILayout.Foldout(_selectPrototypeFoldout, "Groups");

            if (_selectPrototypeFoldout)
            {
                ++EditorGUI.indentLevel;
                _iconStackEditor.OnGUI(Data.GroupList, typeof(Runtime.Core.SelectionDatas.Group.Group));
                --EditorGUI.indentLevel;
            }
        }

        private static void SetSelectedAllPrototypes(IShowIcon icon)
        {
            var group = (Runtime.Core.SelectionDatas.Group.Group)icon;

            group.PrototypeList.ForEach(proto => proto.Selected = group.Selected);
        }

        private IShowIcon AddGroup(Object obj)
        {
            var group = (Runtime.Core.SelectionDatas.Group.Group)obj;

            if (Data.GroupList.Contains(group) == false)
            {
                Data.GroupList.Add(group);
                return group;
            }

            return null;
        }

        private void DrawIcon(Event e, IShowIcon icon, Rect iconRect, Color textColor, Color rectColor)
        {
            GUIStyle labelTextForIcon = CustomEditorGUILayout.GetStyle(StyleName.LabelTextForIcon);

            var group = (Runtime.Core.SelectionDatas.Group.Group)icon;

            EditorGUI.DrawRect(iconRect, rectColor);

            // Prefab preview 
            if (e.type == EventType.Repaint)
            {
                var previewRect = new Rect(iconRect.x + 2, iconRect.y + 2, iconRect.width - 4, iconRect.width - 4);
                var dimmedColor = new Color(0.4f, 0.4f, 0.4f, 1.0f);

                Rect[] icons =
                {
                    new(previewRect.x, previewRect.y, previewRect.width / 2 - 1, previewRect.height / 2 - 1), new(
                        previewRect.x + previewRect.width / 2, previewRect.y, previewRect.width / 2,
                        previewRect.height / 2 - 1),
                    new(previewRect.x, previewRect.y + previewRect.height / 2, previewRect.width / 2 - 1,
                        previewRect.height / 2),
                    new(previewRect.x + previewRect.width / 2, previewRect.y + previewRect.height / 2,
                        previewRect.width / 2, previewRect.height / 2)
                };

                var textures = new Texture2D[4];

                for (int i = 0, j = 0; i < group.PrototypeList.Count && j < 4; i++)
                {
                    if (group.PrototypeList[i].PrototypeObject != null)
                    {
                        textures[j] = group.PrototypeList[i].PreviewTexture;
                        j++;
                    }
                }

                for (var i = 0; i < 4; i++)
                {
                    if (textures[i] != null)
                    {
                        EditorGUI.DrawPreviewTexture(icons[i], textures[i]);
                    }
                    else
                    {
                        EditorGUI.DrawRect(icons[i], dimmedColor);
                    }
                }

                labelTextForIcon.normal.textColor = textColor;
                labelTextForIcon.Draw(iconRect,
                    SelectedWindowUtility.GetShortNameForIcon(group.name, _iconStackEditor.IconWidth), false, false,
                    false, false);
            }
        }

        private void SetIconColor(IShowIcon icon, out Color textColor, out Color rectColor)
        {
            var group = (Runtime.Core.SelectionDatas.Group.Group)icon;
            textColor = EditorColors.Instance.LabelColor;

            if (group.Renaming)
            {
                rectColor = EditorColors.Instance.orangeNormal.WithAlpha(0.3f);

                if (EditorGUIUtility.isProSkin)
                {
                    textColor = EditorColors.Instance.orangeNormal;
                }
                else
                {
                    textColor = EditorColors.Instance.orangeDark;
                }
            }

            else if (group.Selected)
            {
                rectColor = EditorColors.Instance.ToggleButtonActiveColor;
            }
            else
            {
                rectColor = EditorColors.Instance.ToggleButtonInactiveColor;
            }
        }
    }
}
#endif
