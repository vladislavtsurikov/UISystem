using System;
using UnityEngine;
using Action = VladislavTsurikov.ActionFlow.Runtime.Actions.Action;

namespace VladislavTsurikov.EntityDataAction.Runtime.Core
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

        protected T Get<T>() where T : ComponentData
        {
            if (Entity == null)
            {
                return null;
            }

            return (T)Entity.Data.GetElement(typeof(T));
        }

        protected TComponent[] GetComponentsInChildren<TComponent>(bool includeInactive) where TComponent : MonoBehaviour
        {
            if (Entity == null)
            {
                return Array.Empty<TComponent>();
            }

            return Entity.GetComponentsInChildren<TComponent>(includeInactive);
        }
    }
}
