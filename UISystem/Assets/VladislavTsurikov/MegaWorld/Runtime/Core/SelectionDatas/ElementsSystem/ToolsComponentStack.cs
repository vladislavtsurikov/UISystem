using System;
using System.Linq;
using OdinSerializer;
using VladislavTsurikov.AttributeUtility.Runtime;
using VladislavTsurikov.ComponentStack.Runtime.AdvancedComponentStack;
using VladislavTsurikov.ComponentStack.Runtime.Core;
using Runtime_Core_Component = VladislavTsurikov.ComponentStack.Runtime.Core.Component;

namespace VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.ElementsSystem
{
    [Serializable]
    public class ToolsComponentStack : ComponentStackSupportSameType<ToolComponentStack>
    {
        [OdinSerialize]
        private Type _addElementsAttributeType;

        [OdinSerialize]
        private Type _prototypeType;

        protected override void OnSetup()
        {
            _addElementsAttributeType = (Type)SetupData[0];
            _prototypeType = (Type)SetupData[1];
        }

        protected override void OnCreateElements()
        {
            foreach (Type type in AllToolTypes.TypeList)
            {
                ToolComponentStack toolComponentStack = GetToolStackElement(type);

                if (toolComponentStack == null)
                {
                    toolComponentStack = Create(typeof(ToolComponentStack));
                    toolComponentStack.ToolType = type;
                }

                foreach (Attribute attribute in type.GetAttributes(_addElementsAttributeType))
                {
                    var addComponentsAttribute = (AddComponentsAttribute)attribute;

                    if (!addComponentsAttribute.PrototypeTypes.Contains(_prototypeType))
                    {
                        continue;
                    }

                    toolComponentStack.ComponentStack.CreateIfMissingType(addComponentsAttribute.Types);
                }
            }
        }

        public override void OnRemoveInvalidElements()
        {
            for (var i = ElementList.Count - 1; i >= 0; i--)
            {
                if (!ElementList[i].DeleteElement())
                {
                    Remove(i);
                }
            }

            foreach (Type toolType in AllToolTypes.TypeList)
            {
                ToolComponentStack toolComponentStack = GetToolStackElement(toolType);

                if (toolComponentStack == null)
                {
                    continue;
                }

                for (var i = toolComponentStack.ComponentStack.ElementList.Count - 1; i >= 0; i--)
                {
                    if (toolComponentStack.ComponentStack.ElementList[i] == null ||
                        !IsElementBelongsToTool(toolComponentStack.ComponentStack.ElementList[i], toolType))
                    {
                        Remove(i);
                    }
                }
            }
        }

        private bool IsElementBelongsToTool(Element element, Type toolType)
        {
            foreach (Attribute attribute in toolType.GetAttributes(_addElementsAttributeType))
            {
                var addComponentsAttribute = (AddComponentsAttribute)attribute;

                if (!addComponentsAttribute.PrototypeTypes.Contains(_prototypeType))
                {
                    continue;
                }

                if (addComponentsAttribute.Types.Contains(element.GetType()))
                {
                    return true;
                }
            }

            return false;
        }

        public Runtime_Core_Component GetElement(Type toolType, Type typeElement)
        {
            foreach (ToolComponentStack globalToolSettings in ElementList)
            {
                if (globalToolSettings.ToolType == toolType)
                {
                    foreach (Component element in globalToolSettings.ComponentStack.ElementList)
                    {
                        if (element.GetType() == typeElement)
                        {
                            return element;
                        }
                    }
                }
            }

            return null;
        }

        public ToolComponentStack GetToolStackElement(Type toolType)
        {
            foreach (ToolComponentStack element in _elementList)
            {
                if (element.ToolType == toolType)
                {
                    return element;
                }
            }

            return null;
        }
    }
}
