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
using VladislavTsurikov.ReflectionUtility;
using VladislavTsurikov.UnityUtility.Editor;
using Runtime_Core_Component = VladislavTsurikov.ComponentStack.Runtime.Core.Component;

namespace VladislavTsurikov.MegaWorld.Editor.Core.SelectionDatas.Group.Prototypes
{
    public class ClipboardPrototype : ClipboardObject
    {
        private readonly ClipboardStack<Runtime_Core_Component> _clipboardPrototypeComponentStack;
        private readonly ClipboardStack<Runtime_Core_Component> _clipboardPrototypeGeneralComponentStack;

        public ClipboardPrototype(Type toolType, Type prototypeType) : base(prototypeType, toolType)
        {
            _clipboardPrototypeGeneralComponentStack = new ClipboardStack<Runtime_Core_Component>();
            _clipboardPrototypeComponentStack = new ClipboardStack<Runtime_Core_Component>();
        }

        protected override void Copy(List<IHasElementStack> objects)
        {
            _clipboardPrototypeGeneralComponentStack.Copy(GetGeneralStacks(objects));
            _clipboardPrototypeComponentStack.Copy(GetStacks(objects));
        }

        protected override void ClipboardAction(List<IHasElementStack> objects, bool paste)
        {
            _clipboardPrototypeGeneralComponentStack.ClipboardAction(GetGeneralStacks(objects), paste);
            _clipboardPrototypeComponentStack.ClipboardAction(GetStacks(objects), paste);
        }

        protected override void ClipboardAction(List<IHasElementStack> objects, Type settingsType, bool paste)
        {
            _clipboardPrototypeGeneralComponentStack.ClipboardAction(GetGeneralStacks(objects), settingsType, paste);
            _clipboardPrototypeComponentStack.ClipboardAction(GetStacks(objects), settingsType, paste);
        }

        private List<Runtime_Core_Component> GetAllCopiedComponent()
        {
            var copiedComponents =
                (List<Runtime_Core_Component>)_clipboardPrototypeGeneralComponentStack.CopiedComponentList;
            copiedComponents.AddRange(_clipboardPrototypeComponentStack.CopiedComponentList);
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

        public void PrototypeMenu(GenericMenu menu, SelectedData selectedData)
        {
            if (AllowMenu(selectedData))
            {
                menu.AddSeparator("");

                if (selectedData.HasOneSelectedPrototype())
                {
                    menu.AddItem(new GUIContent("Copy All Settings"), false, ContextMenuUtility.ContextMenuCallback,
                        new Action(() =>
                            Copy(new List<IHasElementStack> { selectedData.SelectedPrototype })));
                }

                if (GetAllCopiedComponent().Count != 0)
                {
                    menu.AddItem(new GUIContent("Paste All Settings"), false, ContextMenuUtility.ContextMenuCallback,
                        new Action(() =>
                            ClipboardAction(new List<IHasElementStack>(selectedData.SelectedPrototypeList), true)));

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
                            new Action(() =>
                                ClipboardAction(new List<IHasElementStack>(selectedData.SelectedPrototypeList),
                                    component.GetType(), true)));
                    }
                }
            }
        }

        private bool AllowMenu(SelectedData selectedData)
        {
            if (selectedData.HasOneSelectedPrototype())
            {
                if (selectedData.SelectedPrototype.ComponentStackManager.GetAllElementTypes(ToolType, PrototypeType)
                        .Count == 0)
                {
                    return false;
                }
            }

            if (selectedData.HasOneSelectedPrototype() || GetAllCopiedComponent().Count != 0)
            {
                return true;
            }

            return false;
        }
    }
}
#endif
