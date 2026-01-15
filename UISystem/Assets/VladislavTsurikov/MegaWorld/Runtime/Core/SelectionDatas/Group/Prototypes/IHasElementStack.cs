using System;
using VladislavTsurikov.Nody.Runtime.Core;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.ElementsSystem;

namespace VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes
{
    public interface IHasElementStack
    {
        ComponentStackManager ComponentStackManager { get; }

        public void SetupComponentStack();

        public Node GetElement(Type elementType) =>
            ComponentStackManager.GeneralComponentStack.GetElement(elementType);

        public Node GetElement(Type toolType, Type elementType) =>
            ComponentStackManager.ToolsComponentStack.GetElement(elementType, toolType);
    }
}
