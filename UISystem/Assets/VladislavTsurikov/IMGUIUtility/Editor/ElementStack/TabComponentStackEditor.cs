#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.AttributeUtility.Runtime;
using VladislavTsurikov.ComponentStack.Editor.Core;
using VladislavTsurikov.ComponentStack.Runtime.AdvancedComponentStack;
using VladislavTsurikov.ComponentStack.Runtime.Core;
using VladislavTsurikov.ReflectionUtility;
using VladislavTsurikov.UnityUtility.Editor;
using Runtime_Core_Component = VladislavTsurikov.ComponentStack.Runtime.Core.Component;

namespace VladislavTsurikov.IMGUIUtility.Editor.ElementStack
{
    public class TabComponentStackEditor<T, N> : ComponentStackEditor<T, N>
        where T : Runtime_Core_Component
        where N : IMGUIElementEditor
    {
        protected readonly TabStackEditor _tabStackEditor;

        public TabComponentStackEditor(AdvancedComponentStack<T> stack) : base(stack) =>
            _tabStackEditor = new TabStackEditor(stack.ReorderableElementList, true, false)
            {
                AddCallback = ShowAddManu,
                AddTabMenuCallback = TabMenu,
                HappenedMoveCallback = HappenedMove,
                TabWidthFromName = true
            };

        protected AdvancedComponentStack<T> AdvancedComponentStack => (AdvancedComponentStack<T>)Stack;

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

                if (settingsType.GetAttribute<PersistentComponentAttribute>() != null ||
                    settingsType.GetAttribute<DontShowInAddMenuAttribute>() != null)
                {
                    continue;
                }

                var context = settingsType.GetAttribute<NameAttribute>().Name;

                if (Stack is ComponentStackSupportSameType<T> componentStackWithSameTypes)
                {
                    menu.AddItem(new GUIContent(context), false,
                        () => componentStackWithSameTypes.CreateComponent(settingsType));
                }
                else if (Stack is ComponentStackOnlyDifferentTypes<T> componentStackWithDifferentTypes)
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
