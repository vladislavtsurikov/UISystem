using VladislavTsurikov.ComponentStack.Runtime.Core;
using VladislavTsurikov.EntityDataActionFramework;

namespace Plugins.VladislavTsurikov.EntiryDataAction.Runtime
{
    public abstract class ComponentData : Component
    {
        public Entity Entity
        {
            get
            {
                EntityDataCollection collection = Stack as EntityDataCollection;
                if (collection == null)
                {
                    return null;
                }

                return collection.Entity;
            }
        }
    }
}
