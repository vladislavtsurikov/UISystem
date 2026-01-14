using System;
using Plugins.VladislavTsurikov.EntiryDataAction.Runtime;
using UnityEngine;
using VladislavTsurikov.ActionFlow.Runtime.Actions;
using Action = VladislavTsurikov.ActionFlow.Runtime.Actions.Action;

namespace VladislavTsurikov.EntityDataActionFramework
{
    public abstract class EntityAction : Action
    {
        public Entity Entity
        {
            get
            {
                EntityActionCollection collection = Stack as EntityActionCollection;
                if (collection == null)
                {
                    return null;
                }

                return collection.Entity;
            }
        }

        protected T Get<T>() where T : NodeData
        {
            if (Entity == null)
            {
                return null;
            }

            return (T)Entity.Data.GetElement(typeof(T));
        }

        protected TComponent[] GetComponentsInChildren<TComponent>(bool includeInactive) where TComponent : Node
        {
            if (Entity == null)
            {
                return Array.Empty<TComponent>();
            }

            return Entity.GetComponentsInChildren<TComponent>(includeInactive);
        }
    }
}
