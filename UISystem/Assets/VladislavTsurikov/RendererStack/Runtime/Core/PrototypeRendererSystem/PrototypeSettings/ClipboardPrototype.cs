#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.AttributeUtility.Runtime;
using VladislavTsurikov.ComponentStack.Runtime.AdvancedComponentStack;
using VladislavTsurikov.ReflectionUtility;
using VladislavTsurikov.UnityUtility.Editor;

namespace VladislavTsurikov.RendererStack.Runtime.Core.PrototypeRendererSystem.PrototypeSettings
{
    public class ClipboardPrototype
    {
        private readonly ClipboardStack<PrototypeComponent> _clipboardStack = new();

        public void PrototypeMenu(GenericMenu menu, SelectedData selectedData)
        {
            if (selectedData.HasOneSelectedProto() || _clipboardStack.CopiedComponentList.Count != 0)
            {
                menu.AddSeparator("");
            }

            if (selectedData.HasOneSelectedProto())
            {
                menu.AddItem(new GUIContent("Copy All Settings"), false, ContextMenuUtility.ContextMenuCallback,
                    new Action(() => _clipboardStack.Copy(GetStacks(selectedData.SelectedProtoList))));
            }

            if (_clipboardStack.CopiedComponentList.Count != 0)
            {
                menu.AddItem(new GUIContent("Paste All Settings"), false, ContextMenuUtility.ContextMenuCallback,
                    new Action(() => _clipboardStack.ClipboardAction(GetStacks(selectedData.SelectedProtoList), true)));

                foreach (PrototypeComponent baseSettings in _clipboardStack.CopiedComponentList)
                {
                    var name = baseSettings.GetType().GetAttribute<NameAttribute>().Name;
                    menu.AddItem(new GUIContent("Paste Settings/" + name), false,
                        ContextMenuUtility.ContextMenuCallback,
                        new Action(() => _clipboardStack.ClipboardAction(GetStacks(selectedData.SelectedProtoList),
                            baseSettings.GetType(), true)));
                }
            }
        }

        private List<AdvancedComponentStack<PrototypeComponent>> GetStacks(List<Prototype> objects)
        {
            var stacks = new List<AdvancedComponentStack<PrototypeComponent>>();

            foreach (Prototype obj in objects)
            {
                stacks.Add(obj.PrototypeComponentStack);
            }

            return stacks;
        }
    }
}
#endif
