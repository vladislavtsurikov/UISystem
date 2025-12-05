using System.Linq;
using OdinSerializer;
using VladislavTsurikov.AttributeUtility.Runtime;
using VladislavTsurikov.ComponentStack.Runtime.AdvancedComponentStack;
using VladislavTsurikov.ComponentStack.Runtime.Core;

namespace VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.DefaultComponentsSystem
{
    public class DefaultGroupComponentStack : ComponentStackOnlyDifferentTypes<Component>
    {
        [OdinSerialize]
        private Group _group;

        protected override void OnSetup() => _group = (Group)SetupData[0];

        protected override void OnCreateElements()
        {
            AddDefaultGroupComponentsAttribute addDefaultGroupComponentsAttribute =
                _group.PrototypeType.GetAttribute<AddDefaultGroupComponentsAttribute>();

            if (addDefaultGroupComponentsAttribute == null)
            {
                return;
            }

            CreateIfMissingType(addDefaultGroupComponentsAttribute.Types);
        }

        public override void OnRemoveInvalidElements()
        {
            AddDefaultGroupComponentsAttribute addDefaultGroupComponentsAttribute =
                _group.PrototypeType.GetAttribute<AddDefaultGroupComponentsAttribute>();

            if (addDefaultGroupComponentsAttribute == null)
            {
                RemoveAll();
                return;
            }

            for (var i = ElementList.Count - 1; i >= 0; i--)
            {
                if (!addDefaultGroupComponentsAttribute.Types.Contains(ElementList[i].GetType()))
                {
                    Remove(i);
                }
            }
        }
    }
}
