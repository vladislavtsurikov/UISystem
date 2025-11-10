#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.ColorUtility.Runtime;
using VladislavTsurikov.IMGUIUtility.Editor;
using VladislavTsurikov.IMGUIUtility.Editor.ElementStack.IconStack;
using VladislavTsurikov.IMGUIUtility.Runtime.ElementStack.IconStack;
using VladislavTsurikov.RendererStack.Runtime.Core.PrototypeRendererSystem;
using VladislavTsurikov.RendererStack.Runtime.Core.PrototypeRendererSystem.PrototypeSettings;
using VladislavTsurikov.UnityUtility.Editor;
using Object = UnityEngine.Object;

namespace VladislavTsurikov.RendererStack.Editor.Core.PrototypeRendererSystem.SelectionData
{
    public class PrototypeSelectedEditor
    {
        private readonly ClipboardPrototype _clipboardPrototype = new();
        private readonly IconStackEditor _iconStackEditor = new(false);
        private readonly Runtime.Core.PrototypeRendererSystem.SelectionData _selectionData;

        public PrototypeSelectedEditor(Runtime.Core.PrototypeRendererSystem.SelectionData selectionData)
        {
            _selectionData = selectionData;

            _iconStackEditor.AddIconCallback = selectionData.AddMissingPrototype;
            _iconStackEditor.IconMenuCallback = PrototypeMenu;
            _iconStackEditor.ZeroIconsWarning = "Has no prototypes";
            _iconStackEditor.IconColor = SetIconColor;
        }

        public void OnGUI()
        {
            Group selectedGroup = _selectionData.SelectedData.GetLastGroup();

            if (selectedGroup != null)
            {
                List<Prototype> protoList = _selectionData.SelectedData.GetLastGroup().PrototypeList;
                _iconStackEditor.OnGUI(protoList, _selectionData.PrototypeType);
            }
            else
            {
                _iconStackEditor.OnGUI();
            }
        }

        private GenericMenu PrototypeMenu(IShowIcon selectedIcon)
        {
            var menu = new GenericMenu();

            var proto = (Prototype)selectedIcon;

            Object prototypeObject = proto.PrototypeObject;

            if (prototypeObject != null)
            {
                menu.AddItem(new GUIContent("Reveal in Project"), false, ContextMenuUtility.ContextMenuCallback,
                    new Action(() => EditorGUIUtility.PingObject(prototypeObject)));
                menu.AddSeparator("");
            }

            menu.AddItem(new GUIContent("Delete"), false, ContextMenuUtility.ContextMenuCallback,
                new Action(() => _selectionData.RemovePrototypes(_selectionData.SelectedData.SelectedProtoList)));
            menu.AddItem(new GUIContent("Delete From All Scenes"), false, ContextMenuUtility.ContextMenuCallback,
                new Action(() => _selectionData.RemovePrototypes(_selectionData.SelectedData.SelectedProtoList, true)));

            menu.AddSeparator("");

            menu.AddItem(new GUIContent("Active"), proto.Active, ContextMenuUtility.ContextMenuCallback,
                new Action(() => _selectionData.SelectedData.SelectedProtoList.ForEach(localProto =>
                {
                    localProto.Active = !localProto.Active;
                })));

            _clipboardPrototype.PrototypeMenu(menu, _selectionData.SelectedData);
            GroupMenu(menu, _selectionData);

            return menu;
        }

        private void GroupMenu(GenericMenu menu, Runtime.Core.PrototypeRendererSystem.SelectionData selectionData)
        {
            if (selectionData.GroupList.Count == 1)
            {
                return;
            }

            menu.AddSeparator("");

            Group selectedGroup = selectionData.SelectedData.GetLastGroup();

            for (var i = 0; i < selectionData.GroupList.Count; i++)
            {
                Group group = selectionData.GroupList[i];

                menu.AddItem(new GUIContent("Group/" + group.Name), selectedGroup == group,
                    ContextMenuUtility.ContextMenuCallback, new Action(() =>
                        ChangePrototypeGroup(group, selectionData)));
            }
        }

        private void ChangePrototypeGroup(Group group, Runtime.Core.PrototypeRendererSystem.SelectionData selectionData)
        {
            Group selectedGroup = selectionData.SelectedData.GetLastGroup();

            if (selectedGroup == group)
            {
                return;
            }

            foreach (Prototype proto in selectionData.SelectedData.SelectedProtoList)
            {
                selectedGroup.PrototypeList.Remove(proto);

                if (!group.PrototypeList.Contains(proto))
                {
                    group.PrototypeList.Add(proto);
                }
            }
        }

        private void SetIconColor(IShowIcon icon, out Color textColor, out Color rectColor)
        {
            var proto = (Prototype)icon;

            textColor = EditorColors.Instance.LabelColor;

            if (proto.PrototypeConsole.HasError())
            {
                textColor = EditorColors.Instance.LabelColor;

                if (icon.Selected)
                {
                    rectColor = icon.IsRedIcon
                        ? EditorColors.Instance.redNormal
                        : EditorColors.Instance.ToggleButtonActiveColor;
                }
                else
                {
                    rectColor = icon.IsRedIcon
                        ? EditorColors.Instance.redDark
                        : EditorColors.Instance.ToggleButtonInactiveColor;
                }
            }
            else if (!proto.Active)
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
            else if (proto.Selected)
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
