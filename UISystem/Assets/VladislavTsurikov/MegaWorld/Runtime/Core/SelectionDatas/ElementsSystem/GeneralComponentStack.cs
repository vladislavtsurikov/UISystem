using System;
using System.Linq;
using OdinSerializer;
using VladislavTsurikov.AttributeUtility.Runtime;
using VladislavTsurikov.ComponentStack.Runtime.AdvancedComponentStack;
using VladislavTsurikov.ComponentStack.Runtime.Core;

namespace VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.ElementsSystem
{
    public class GeneralComponentStack : ComponentStackOnlyDifferentTypes<Component>
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
            foreach (Attribute attribute in type.GetAttributes(_addElementsAttributeType))
            {
                var addComponentsAttribute = (AddComponentsAttribute)attribute;

                if (!addComponentsAttribute.PrototypeTypes.Contains(_prototypeType))
                {
                    continue;
                }

                CreateIfMissingType(addComponentsAttribute.Types);
            }
        }

        public override void OnRemoveInvalidElements()
        {
            for (var i = ElementList.Count - 1; i >= 0; i--)
            {
                var found = false;

                foreach (Type toolType in AllToolTypes.TypeList)
                {
                    if (IsElementBelongsToTool(ElementList[i], toolType))
                    {
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    Remove(i);
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
    }
}
