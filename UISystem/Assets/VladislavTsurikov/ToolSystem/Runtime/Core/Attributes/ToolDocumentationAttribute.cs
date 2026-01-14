using System;

namespace VladislavTsurikov.ToolSystem.Runtime.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class ToolDocumentationAttribute : Attribute
    {
        public string Url { get; }

        public ToolDocumentationAttribute(string url)
        {
            Url = url;
        }
    }
}
