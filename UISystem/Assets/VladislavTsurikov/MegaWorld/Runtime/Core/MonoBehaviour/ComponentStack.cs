using System.Linq;
using VladislavTsurikov.AttributeUtility.Runtime;
using VladislavTsurikov.Nody.Runtime.AdvancedNodeStack;
using VladislavTsurikov.Nody.Runtime.Core;

namespace VladislavTsurikov.MegaWorld.Runtime.Core.MonoBehaviour
{
    public class ComponentStack : NodeStackOnlyDifferentTypes<Node>
    {
        private UnityEngine.MonoBehaviour _tool;

        protected override void OnSetup() => _tool = (UnityEngine.MonoBehaviour)SetupData[0];

        protected override void OnCreateElements()
        {
            var addMonoBehaviourComponentsAttribute =
                (AddMonoBehaviourComponentsAttribute)_tool.GetType()
                    .GetAttribute(typeof(AddMonoBehaviourComponentsAttribute));

            if (addMonoBehaviourComponentsAttribute == null)
            {
                return;
            }

            CreateIfMissingType(addMonoBehaviourComponentsAttribute.Types);
        }

        public override void OnRemoveInvalidElements()
        {
            for (var i = ElementList.Count - 1; i >= 0; i--)
            {
                var addMonoBehaviourComponentsAttribute =
                    (AddMonoBehaviourComponentsAttribute)_tool.GetType()
                        .GetAttribute(typeof(AddMonoBehaviourComponentsAttribute));

                if (addMonoBehaviourComponentsAttribute == null)
                {
                    Remove(i);
                    continue;
                }

                if (!addMonoBehaviourComponentsAttribute.Types.Contains(ElementList[i].GetType()))
                {
                    Remove(i);
                }
            }
        }
    }
}
