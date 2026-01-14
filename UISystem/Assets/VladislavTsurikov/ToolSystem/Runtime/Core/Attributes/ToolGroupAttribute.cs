using System;

namespace VladislavTsurikov.ToolSystem.Runtime.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class ToolGroupAttribute : Attribute
    {
        public string GroupName { get; }

        public ToolGroupAttribute(string groupName)
        {
            GroupName = groupName;
        }
    }
}
