using System;
using Cysharp.Threading.Tasks;
using OdinSerializer;
using VladislavTsurikov.Nody.Runtime.AdvancedNodeStack;
using VladislavTsurikov.Nody.Runtime.Core;

namespace VladislavTsurikov.RendererStack.Runtime.Core.GlobalSettings
{
    public class RendererGlobalComponentStack : Node
    {
        [OdinSerialize]
        public NodeStackOnlyDifferentTypes<GlobalComponent> ComponentStack = new();

        public Type RendererType;

        protected override void SetupComponent(object[] setupData = null) => ComponentStack.Setup();

        public override bool DeleteElement() => RendererType != null;
    }
}
