#if UNITY_EDITOR
using System;
using VladislavTsurikov.AttributeUtility.Runtime;
using VladislavTsurikov.Nody.Runtime.AdvancedNodeStack;

namespace VladislavTsurikov.MegaWorld.Editor.Core.SelectionDatas.ResourceController
{
    public class ResourceControllerEditorStack : NodeStackOnlyDifferentTypes<ResourceControllerEditor>
    {
        protected override void OnCreateElements() => CreateAllElementTypes();

        public ResourceControllerEditor GetResourceControllerEditor(Type prototypeType)
        {
            foreach (ResourceControllerEditor editor in _elementList)
            {
                var resourceControllerAttribute =
                    (ResourceControllerAttribute)editor.GetType().GetAttribute(typeof(ResourceControllerAttribute));

                if (resourceControllerAttribute.PrototypeType == prototypeType)
                {
                    return editor;
                }
            }

            return null;
        }
    }
}
#endif
