using System.Collections.Generic;

namespace VladislavTsurikov.Utility.Runtime
{
    public static class StackExtensions
    {
        public static void Remove<T>(this Stack<T> stack, T item)
        {
            if (stack.Count == 0 || !stack.Contains(item))
            {
                return;
            }

            var temp = new Stack<T>();

            while (stack.Count > 0)
            {
                T top = stack.Pop();
                if (EqualityComparer<T>.Default.Equals(top, item))
                {
                    break;
                }

                temp.Push(top);
            }

            while (temp.Count > 0)
            {
                stack.Push(temp.Pop());
            }
        }
    }
}
