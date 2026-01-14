#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.AttributeUtility.Runtime;
using VladislavTsurikov.Nody.Editor.Core;
using VladislavTsurikov.Nody.Runtime.AdvancedNodeStack;
using VladislavTsurikov.Nody.Runtime.Core;
using VladislavTsurikov.ReflectionUtility;
using VladislavTsurikov.UnityUtility.Editor;

namespace VladislavTsurikov.IMGUIUtility.Editor.ElementStack
{
    public class TabComponentStackEditor<T, N> : NodeStackEditor<T, N>
        where T : Node
        where N : IMGUIElementEditor
    {
        protected readonly TabStackEditor _tabStackEditor;

        public TabComponentStackEditor(AdvancedNodeStack<T> stack) : base(stack) =>
            _tabStackEditor = new TabStackEditor(stack.List, true, false)
            {
                AddCallback = ShowAddManu,
                AddTabMenuCallback = TabMenu,
                HappenedMoveCallback = HappenedMove,
                TabWidthFromName = true
            };

        protected AdvancedNodeStack<T> AdvancedNodeStack => (AdvancedNodeStack<T>)Stack;

        public virtual void OnTabStackGUI() => _tabStackEditor.OnGUI();

        public void DrawSelectedSettings()
        {
            if (Stack.IsDirty)
            {
                Stack.RemoveInvalidElements();
                RefreshEditors();
                Stack.IsDirty = false;
            }

            if (Stack.ElementList.Count == 0)
            {
                return;
            }

            OnSelectedComponentGUI();
        }

        protected virtual void OnSelectedComponentGUI()
        {
            if (Stack.SelectedElement == null)
            {
                return;
            }

            SelectedEditor?.OnGUI();
        }

        protected virtual GenericMenu TabMenu(int currentTabIndex)
        {
            var menu = new GenericMenu();

            if (Stack.ElementList.Count > 1)
            {
                menu.AddItem(new GUIContent("Delete"), false, ContextMenuUtility.ContextMenuCallback,
                    new Action(() => { Stack.Remove(currentTabIndex); }));
            }
            else
            {
                menu.AddDisabledItem(new GUIContent("Delete"));
            }

            return menu;
        }

        protected virtual void ShowAddManu()
        {
            var menu = new GenericMenu();

            foreach (KeyValuePair<Type, Type> item in AllEditorTypes<T>.Types)
            {
                Type settingsType = item.Key;

                if (settingsType.GetAttribute(typeof(DontCreateAttribute)) != null)
                {
                    continue;
                }

                if (settingsType.GetAttribute<PersistentNodeAttribute>() != null ||
                    settingsType.GetAttribute<DontShowInAddMenuAttribute>() != null)
                {
                    continue;
                }

                var context = settingsType.GetAttribute<NameAttribute>().Name;

                if (Stack is NodeStackSupportSameType<T> componentStackWithSameTypes)
                {
                    menu.AddItem(new GUIContent(context), false,
                        () => componentStackWithSameTypes.CreateNode(settingsType));
                }
                else if (Stack is NodeStackOnlyDifferentTypes<T> componentStackWithDifferentTypes)
                {
                    var exists = componentStackWithDifferentTypes.HasType(settingsType);

                    if (!exists)
                    {
                        menu.AddItem(new GUIContent(context), false,
                            () => componentStackWithDifferentTypes.CreateIfMissingType(settingsType));
                    }
                    else
                    {
                        menu.AddDisabledItem(new GUIContent(context));
                    }
                }
            }

            menu.ShowAsContext();
        }

        private void HappenedMove() => Stack.IsDirty = true;
    }
}
#endif
