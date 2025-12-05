using System;
using VladislavTsurikov.AttributeUtility.Runtime;
using VladislavTsurikov.ComponentStack.Runtime.AdvancedComponentStack;

namespace VladislavTsurikov.RendererStack.Runtime.Core.PrototypeRendererSystem.PrototypeSettings
{
    public class PrototypeComponentStack : ComponentStackOnlyDifferentTypes<PrototypeComponent>
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
                if (type.GetAttribute<PersistentComponentAttribute>() != null)
                {
                    CreateIfMissingType(type);
                }
            }
        }
    }
}
