using System;

namespace VladislavTsurikov.ComponentStack.Runtime.Core
{
    public static class ComponentStackExtensions
    {
        public static void ForEach<T>(this ComponentStack<T> stack, Action<T> action) where T : Component
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
