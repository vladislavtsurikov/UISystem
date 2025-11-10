#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.AttributeUtility.Runtime;
using VladislavTsurikov.ComponentStack.Runtime.AdvancedComponentStack;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.ElementsSystem;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Utility;
using VladislavTsurikov.ReflectionUtility;
using VladislavTsurikov.UnityUtility.Editor;
using Runtime_Core_Component = VladislavTsurikov.ComponentStack.Runtime.Core.Component;

namespace VladislavTsurikov.MegaWorld.Editor.Core.SelectionDatas.Group
{
    public class ClipboardGroup : ClipboardObject
    {
        private readonly ClipboardStack<Runtime_Core_Component> _clipboardGroupComponentStack;
        private readonly ClipboardStack<Runtime_Core_Component> _clipboardGroupGeneralComponentStack;

        public ClipboardGroup(Type toolType, Type prototypeType) : base(prototypeType, toolType)
        {
            _clipboardGroupGeneralComponentStack = new ClipboardStack<Runtime_Core_Component>();
            _clipboardGroupComponentStack = new ClipboardStack<Runtime_Core_Component>();
        }

        protected override void Copy(List<IHasElementStack> objects)
        {
            _clipboardGroupGeneralComponentStack.Copy(GetGeneralStacks(objects));
            _clipboardGroupComponentStack.Copy(GetStacks(objects));
        }

        protected override void ClipboardAction(List<IHasElementStack> objects, bool paste)
        {
            _clipboardGroupGeneralComponentStack.ClipboardAction(GetGeneralStacks(objects), paste);
            _clipboardGroupComponentStack.ClipboardAction(GetStacks(objects), paste);
        }

        protected override void ClipboardAction(List<IHasElementStack> objects, Type settingsType, bool paste)
        {
            _clipboardGroupGeneralComponentStack.ClipboardAction(GetGeneralStacks(objects), settingsType, paste);
            _clipboardGroupComponentStack.ClipboardAction(GetStacks(objects), settingsType, paste);
        }

        private List<Runtime_Core_Component> GetAllCopiedComponent()
        {
            var copiedComponents =
                (List<Runtime_Core_Component>)_clipboardGroupGeneralComponentStack.CopiedComponentList;
            copiedComponents.AddRange(_clipboardGroupComponentStack.CopiedComponentList);
            return copiedComponents;
        }

        private List<AdvancedComponentStack<Runtime_Core_Component>> GetGeneralStacks(List<IHasElementStack> objects)
        {
            var stacks = new List<AdvancedComponentStack<Runtime_Core_Component>>();

            foreach (IHasElementStack obj in objects)
            {
                stacks.Add(obj.ComponentStackManager.GeneralComponentStack);
            }

            return stacks;
        }

        private List<AdvancedComponentStack<Runtime_Core_Component>> GetStacks(List<IHasElementStack> objects)
        {
            var stacks = new List<AdvancedComponentStack<Runtime_Core_Component>>();

            foreach (IHasElementStack obj in objects)
            foreach (ToolComponentStack toolComponentStack in obj.ComponentStackManager.ToolsComponentStack.ElementList)
            {
                if (toolComponentStack.ToolType == ToolType)
                {
                    stacks.Add(toolComponentStack.ComponentStack);
                }
            }

            return stacks;
        }

        public void GroupMenu(GenericMenu menu, SelectedData selectedData)
        {
            if (AllowMenu(selectedData))
            {
                if (selectedData.HasOneSelectedGroup())
                {
                    menu.AddItem(new GUIContent("Copy All Settings"), false, ContextMenuUtility.ContextMenuCallback,
                        new Action(() =>
                            Copy(new List<IHasElementStack> { selectedData.SelectedGroup })));
                }

                if (GetAllCopiedComponent().Count != 0)
                {
                    menu.AddItem(new GUIContent("Paste All Settings"), false, ContextMenuUtility.ContextMenuCallback,
                        new Action(() =>
                            ClipboardAction(new List<IHasElementStack>(selectedData.SelectedGroupList), true)));

                    foreach (Runtime_Core_Component component in GetAllCopiedComponent())
                    {
                        NameAttribute nameAttribute = component.GetType().GetAttribute<NameAttribute>();

                        if (nameAttribute == null)
                        {
                            Debug.Log("MenuItem is not found for " + component.Name);
                            continue;
                        }

                        menu.AddItem(new GUIContent("Paste Settings/" + nameAttribute.Name), false,
                            ContextMenuUtility.ContextMenuCallback,
                            new Action(() => ClipboardAction(new List<IHasElementStack>(selectedData.SelectedGroupList),
                                component.GetType(), true)));
                    }
                }
            }
        }

        private bool AllowMenu(SelectedData selectedData)
        {
            if (selectedData.HasOneSelectedGroup())
            {
                if (selectedData.SelectedGroup.ComponentStackManager.GetAllElementTypes(ToolType, PrototypeType)
                        .Count == 0)
                {
                    return false;
                }
            }

            if (selectedData.HasOneSelectedGroup())
            {
                return true;
            }

            if (GroupUtility.GetGeneralPrototypeType(selectedData.SelectedGroupList) != null)
            {
                return true;
            }

            return false;
        }
    }
}
#endif
