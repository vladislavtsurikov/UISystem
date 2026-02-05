using UnityEngine;

namespace VladislavTsurikov.UIToolkitUtility.Editor.ElementStack.TabStack
{
    public static class TabExtensions
    {
        public static Tab SetText(this Tab target, string text)
        {
            target.Text = text;
            return target;
        }

        public static Tab SetLabelColor(this Tab target, Color color)
        {
            target.SetLabelColor(color);
            return target;
        }

        public static Tab SetBackgroundColor(this Tab target, Color color)
        {
            target.SetBackgroundColor(color);
            return target;
        }

        public static TabPlus SetText(this TabPlus target, string text)
        {
            target.Text = text;
            return target;
        }
    }
}
