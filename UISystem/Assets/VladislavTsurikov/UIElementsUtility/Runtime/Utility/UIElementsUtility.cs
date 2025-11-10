using System.Collections.Generic;
using System.Linq;
using UnityEngine.UIElements;
using VladislavTsurikov.Utility.Runtime;

namespace VladislavTsurikov.UIElementsUtility.Runtime.Utility
{
    public static class UIElementsUtility
    {
        public static void AddClass(string className, IEnumerable<VisualElement> elements, bool removeNulls = true)
        {
            if (className.IsNullOrEmpty())
            {
                return;
            }

            if (elements == null)
            {
                return;
            }

            if (removeNulls)
            {
                elements = elements.Where(item => item != null);
            }

            foreach (VisualElement element in elements)
            {
                element.AddClass(className);
            }
        }

        public static void RemoveClass(string className, IEnumerable<VisualElement> elements, bool removeNulls = true)
        {
            if (className.IsNullOrEmpty())
            {
                return;
            }

            if (elements == null)
            {
                return;
            }

            if (removeNulls)
            {
                elements = elements.Where(item => item != null);
            }

            foreach (VisualElement element in elements)
            {
                element.RemoveClass(className);
            }
        }
    }
}
