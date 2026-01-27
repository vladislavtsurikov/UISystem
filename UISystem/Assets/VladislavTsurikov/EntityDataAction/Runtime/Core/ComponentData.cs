using VladislavTsurikov.Nody.Runtime.Core;

namespace VladislavTsurikov.EntityDataAction.Runtime.Core
{
    public abstract class ComponentData : Node
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
