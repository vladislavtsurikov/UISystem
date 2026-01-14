using System;
using VladislavTsurikov.AttributeUtility.Runtime;
using VladislavTsurikov.Nody.Runtime.AdvancedNodeStack;

namespace VladislavTsurikov.RendererStack.Runtime.Core.PrototypeRendererSystem.PrototypeSettings
{
    public class PrototypeComponentStack : NodeStackOnlyDifferentTypes<PrototypeComponent>
    {
        internal void CreateAllComponents()
        {
            var rendererType = (Type)SetupData[0];

            foreach (Type type in rendererType.GetAttribute<AddPrototypeComponentsAttribute>().PrototypeSettings)
            {
                CreateIfMissingType(type);
            }
        }

        protected override void OnCreateElements()
        {
            var rendererType = (Type)SetupData[0];

            foreach (Type type in rendererType.GetAttribute<AddPrototypeComponentsAttribute>().PrototypeSettings)
            {
                if (type.GetAttribute<PersistentNodeAttribute>() != null)
                {
                    CreateIfMissingType(type);
                }
            }
        }
    }
}
