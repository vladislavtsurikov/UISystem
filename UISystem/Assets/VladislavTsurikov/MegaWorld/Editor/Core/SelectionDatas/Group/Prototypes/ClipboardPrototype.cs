#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.AttributeUtility.Runtime;
using VladislavTsurikov.Nody.Runtime.AdvancedNodeStack;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.ElementsSystem;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes;
using VladislavTsurikov.ReflectionUtility;
using VladislavTsurikov.UnityUtility.Editor;
using VladislavTsurikov.Nody.Runtime.Core;

namespace VladislavTsurikov.MegaWorld.Editor.Core.SelectionDatas.Group.Prototypes
{
    public class ClipboardPrototype : ClipboardObject
    {
        private readonly ClipboardStack<Node> _clipboardPrototypeComponentStack;
        private readonly ClipboardStack<Node> _clipboardPrototypeGeneralComponentStack;

        public ClipboardPrototype(Type toolType, Type prototypeType) : base(prototypeType, toolType)
        {
            _clipboardPrototypeGeneralComponentStack = new ClipboardStack<Node>();
            _clipboardPrototypeComponentStack = new ClipboardStack<Node>();
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

        private List<Node> GetAllCopiedComponent()
        {
            var copiedComponents =
                (List<Node>)_clipboardPrototypeGeneralComponentStack.CopiedComponentList;
            copiedComponents.AddRange(_clipboardPrototypeComponentStack.CopiedComponentList);
            return copiedComponents;
        }

        private List<AdvancedNodeStack<Node>> GetGeneralStacks(List<IHasElementStack> objects)
        {
            var stacks = new List<AdvancedNodeStack<Node>>();

            foreach (IHasElementStack obj in objects)
            {
                stacks.Add(obj.ComponentStackManager.GeneralComponentStack);
            }

            return stacks;
        }

        private List<AdvancedNodeStack<Node>> GetStacks(List<IHasElementStack> objects)
        {
            var stacks = new List<AdvancedNodeStack<Node>>();

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

                    foreach (Node component in GetAllCopiedComponent())
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
