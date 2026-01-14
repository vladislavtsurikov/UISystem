using System;
using System.Linq;
using VladislavTsurikov.AttributeUtility.Runtime;
using VladislavTsurikov.Nody.Runtime.AdvancedNodeStack;
using Runtime_Core_Component = VladislavTsurikov.Nody.Runtime.Core.Component;

namespace VladislavTsurikov.MegaWorld.Runtime.Core.GlobalSettings.ElementsSystem
{
    [Serializable]
    public class CommonComponentStack : NodeStackOnlyDifferentTypes<Runtime_Core_Component>
    {
        protected override void OnCreateElements()
        {
            foreach (Type type in AllToolTypes.TypeList)
            {
                AddGlobalCommonComponentsAttribute addGlobalCommonComponentsAttribute =
                    type.GetAttribute<AddGlobalCommonComponentsAttribute>();

                if (addGlobalCommonComponentsAttribute == null)
                {
                    continue;
                }

                foreach (Type globalSettingsType in addGlobalCommonComponentsAttribute.Types)
                {
                    CreateIfMissingType(globalSettingsType);
                }
            }
        }

        public override void OnRemoveInvalidElements()
        {
            for (var i = ElementList.Count - 1; i >= 0; i--)
            {
                var find = false;

                foreach (Type type in AllToolTypes.TypeList)
                {
                    var attribute =
                        (AddGlobalCommonComponentsAttribute)type.GetAttribute(
                            typeof(AddGlobalCommonComponentsAttribute));

                    if (attribute == null)
                    {
                        continue;
                    }

                    if (attribute.Types.Contains(ElementList[i].GetType()))
                    {
                        find = true;
                        break;
                    }
                }

                if (!find)
                {
                    Remove(i);
                }
            }
        }
    }
}
