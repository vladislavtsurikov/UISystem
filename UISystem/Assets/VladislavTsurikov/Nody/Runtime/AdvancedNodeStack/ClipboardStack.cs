using System;
using System.Collections.Generic;
using VladislavTsurikov.Nody.Runtime.Core;
using VladislavTsurikov.DeepCopy.Runtime;

namespace VladislavTsurikov.Nody.Runtime.AdvancedNodeStack
{
    public class ClipboardStack<T> where T : Node
    {
        private readonly List<T> _copiedComponentList = new();

        public IReadOnlyList<T> CopiedComponentList => _copiedComponentList;

        public void Copy(List<AdvancedNodeStack<T>> stacks)
        {
            _copiedComponentList.Clear();

            ClipboardAction(stacks, false);
        }

        public void ClipboardAction(List<AdvancedNodeStack<T>> stacks, bool paste)
        {
            foreach (AdvancedNodeStack<T> stack in stacks)
            {
                for (var i = 0; i < stack.ElementList.Count; i++)
                {
                    ClipboardAction(stack, stack.ElementList[i].GetType(), paste);
                }
            }
        }

        public void ClipboardAction(List<AdvancedNodeStack<T>> stacks, Type settingsType, bool paste)
        {
            foreach (AdvancedNodeStack<T> stack in stacks)
            {
                for (var i = 0; i < stack.ElementList.Count; i++)
                {
                    ClipboardAction(stack, settingsType, paste);
                }
            }
        }

        private void ClipboardAction(AdvancedNodeStack<T> stack, Type settingsType, bool paste)
        {
            if (paste)
            {
                T copiedComponent = _copiedComponentList.Find(obj => obj.GetType() == settingsType);

                if (copiedComponent == null)
                {
                    return;
                }

                T component = DeepCopier.Copy(copiedComponent);

                ReplaceElement(stack, component);
            }
            else
            {
                _copiedComponentList.Add(DeepCopier.Copy(stack.GetElement(settingsType)));
            }
        }

        private void ReplaceElement(AdvancedNodeStack<T> stack, T replaceComponent)
        {
            for (var i = 0; i < stack.ElementList.Count; i++)
            {
                Node component = stack.ElementList[i];

                if (component.GetType() == replaceComponent.GetType())
                {
                    stack.ReplaceElement(replaceComponent, i);
                    return;
                }
            }
        }
    }
}
