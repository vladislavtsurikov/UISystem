using System;
using Cysharp.Threading.Tasks;
using OdinSerializer;
using VladislavTsurikov.ComponentStack.Runtime.AdvancedComponentStack;
using VladislavTsurikov.ComponentStack.Runtime.Core;

namespace VladislavTsurikov.RendererStack.Runtime.Core.GlobalSettings
{
    public class RendererGlobalComponentStack : Component
    {
        [OdinSerialize]
        public ComponentStackOnlyDifferentTypes<GlobalComponent> ComponentStack = new();

        public Type RendererType;

        protected override void SetupComponent(object[] setupData = null) => ComponentStack.Setup();

        public override bool DeleteElement() => RendererType != null;
    }
}
