using System;

namespace VladislavTsurikov.UnityUtility.Editor
{
    public class ContextMenuUtility
    {
        public static void ContextMenuCallback(object obj)
        {
            if (obj is Action action)
            {
                action.Invoke();
            }
        }
    }
}
