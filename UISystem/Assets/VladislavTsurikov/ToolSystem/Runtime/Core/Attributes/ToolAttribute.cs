using System;

namespace VladislavTsurikov.ToolSystem.Runtime.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class ToolAttribute : Attribute
    {
        public string Name { get; }
        public string Description { get; }
        public string IconPath { get; }

        public ToolAttribute(string name, string description = "", string iconPath = "")
        {
            Name = name;
            Description = description;
            IconPath = iconPath;
        }
    }
}
