using System;

namespace VladislavTsurikov.IMGUIUtility.Runtime.ElementStack.IconStack
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class MissingIconsWarningAttribute : Attribute
    {
        public readonly string Text;

        public MissingIconsWarningAttribute(string text) => Text = text;
    }
}
