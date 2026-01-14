using VladislavTsurikov.Nody.Runtime.AdvancedNodeStack;
using VladislavTsurikov.EntityDataActionFramework;

namespace Plugins.VladislavTsurikov.EntiryDataAction.Runtime
{
    public sealed class EntityDataCollection : NodeStackOnlyDifferentTypes<ComponentData>
    {
        public Entity Entity { get; internal set; }
    }
}
