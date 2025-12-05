#if UNITY_EDITOR
using System;

namespace VladislavTsurikov.MegaWorld.Editor.Core.SelectionDatas.Group.TemplatesSystem
{
    [AttributeUsage(AttributeTargets.Class)]
    public class TemplateAttribute : Attribute
    {
        public readonly string Name;
        public readonly Type[] SupportedResourceTypes;
        public readonly Type[] ToolTypes;

        internal TemplateAttribute(string name, Type[] toolTypes, Type[] supportedResourceTypes)
        {
            ToolTypes = toolTypes;
            Name = name;
            SupportedResourceTypes = supportedResourceTypes;
        }
    }
}
#endif
