using System;

namespace VladislavTsurikov.Nody.Runtime.Core
{
    public static class ComponentStackExtensions
    {
        public static void ForEach<T>(this NodeStack<T> stack, Action<T> action) where T : Node
        {
            if (stack == null || action == null)
            {
                return;
            }

            foreach (T element in stack.ElementList)
            {
                action(element);
            }
        }
    }
}
