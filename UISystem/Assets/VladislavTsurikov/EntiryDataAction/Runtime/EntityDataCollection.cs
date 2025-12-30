using VladislavTsurikov.ComponentStack.Runtime.AdvancedComponentStack;
using VladislavTsurikov.EntityDataActionFramework;

namespace Plugins.VladislavTsurikov.EntiryDataAction.Runtime
{
    public sealed class EntityDataCollection : ComponentStackOnlyDifferentTypes<ComponentData>
    {
        public Entity Entity { get; internal set; }
    }
}
