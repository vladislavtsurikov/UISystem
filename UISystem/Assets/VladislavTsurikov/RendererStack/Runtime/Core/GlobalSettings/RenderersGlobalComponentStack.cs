using System;
using System.Linq;
using VladislavTsurikov.AttributeUtility.Runtime;
using VladislavTsurikov.ComponentStack.Runtime.AdvancedComponentStack;
using VladislavTsurikov.ReflectionUtility.Runtime;
using VladislavTsurikov.RendererStack.Runtime.Core.RendererSystem;

namespace VladislavTsurikov.RendererStack.Runtime.Core.GlobalSettings
{
    public class RenderersGlobalComponentStack : ComponentStackSupportSameType<RendererGlobalComponentStack>
    {
        protected override void OnCreateElements()
        {
            foreach (Type rendererType in AllTypesDerivedFrom<Renderer>.Types)
            {
                AddGlobalComponentsAttribute addComponentsAttribute =
                    rendererType.GetAttribute<AddGlobalComponentsAttribute>();

                if (addComponentsAttribute == null)
                {
                    continue;
                }

                RendererGlobalComponentStack rendererGlobalComponentStack =
                    GetRendererGlobalComponentStack(rendererType);

                if (rendererGlobalComponentStack == null)
                {
                    rendererGlobalComponentStack = Create(typeof(RendererGlobalComponentStack));
                    rendererGlobalComponentStack.RendererType = rendererType;
                }

                foreach (Type globalSettingsType in addComponentsAttribute.Types)
                {
                    rendererGlobalComponentStack.ComponentStack.CreateIfMissingType(globalSettingsType);
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

            foreach (Type rendererType in AllTypesDerivedFrom<Renderer>.Types)
            {
                RendererGlobalComponentStack rendererGlobalComponentStack =
                    GetRendererGlobalComponentStack(rendererType);

                if (rendererGlobalComponentStack == null)
                {
                    continue;
                }

                for (var i = rendererGlobalComponentStack.ComponentStack.ElementList.Count - 1; i >= 0; i--)
                {
                    AddGlobalComponentsAttribute addComponentsAttribute =
                        rendererType.GetAttribute<AddGlobalComponentsAttribute>();

                    if (addComponentsAttribute == null)
                    {
                        rendererGlobalComponentStack.ComponentStack.Remove(i);
                        continue;
                    }

                    if (!addComponentsAttribute.Types.Contains(rendererGlobalComponentStack.ComponentStack
                            .ElementList[i].GetType()))
                    {
                        rendererGlobalComponentStack.ComponentStack.Remove(i);
                    }
                }
            }
        }

        public GlobalComponent GetElement(Type rendererType, Type type)
        {
            foreach (RendererGlobalComponentStack rendererGlobalComponentStack in ElementList)
            {
                if (rendererGlobalComponentStack.RendererType == rendererType)
                {
                    foreach (GlobalComponent element in rendererGlobalComponentStack.ComponentStack.ElementList)
                    {
                        if (element.GetType() == type)
                        {
                            return element;
                        }
                    }
                }
            }

            return null;
        }

        public RendererGlobalComponentStack GetRendererGlobalComponentStack(Type rendererType)
        {
            foreach (RendererGlobalComponentStack element in _elementList)
            {
                if (element.RendererType == rendererType)
                {
                    return element;
                }
            }

            return null;
        }
    }
}
