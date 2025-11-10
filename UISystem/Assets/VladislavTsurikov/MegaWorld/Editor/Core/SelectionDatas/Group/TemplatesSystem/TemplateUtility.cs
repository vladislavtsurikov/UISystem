#if UNITY_EDITOR
using System;
using System.Linq;
using VladislavTsurikov.AttributeUtility.Runtime;

namespace VladislavTsurikov.MegaWorld.Editor.Core.SelectionDatas.Group.TemplatesSystem
{
    public static class TemplateUtility
    {
        public static bool HasTemplate(Type toolType, Type prototypeType)
        {
            foreach (Type type in AllTemplateTypes.TypeList)
            {
                TemplateAttribute templateAttribute = type.GetAttribute<TemplateAttribute>();

                if (templateAttribute.ToolTypes.Contains(toolType) &&
                    templateAttribute.SupportedResourceTypes.Contains(prototypeType))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
#endif
