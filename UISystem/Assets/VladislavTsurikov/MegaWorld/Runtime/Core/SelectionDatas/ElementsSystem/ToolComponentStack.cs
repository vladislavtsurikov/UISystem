using System;
using VladislavTsurikov.Nody.Runtime.AdvancedNodeStack;
using VladislavTsurikov.Nody.Runtime.Core;

namespace VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.ElementsSystem
{
    public class ToolComponentStack : Node
    {
        public NodeStackOnlyDifferentTypes<Node> ComponentStack = new();

        public Type ToolType;

        protected override void SetupComponent(object[] setupData = null) => ComponentStack.Setup();

        public override bool DeleteElement() => ToolType != null;
    }
}
