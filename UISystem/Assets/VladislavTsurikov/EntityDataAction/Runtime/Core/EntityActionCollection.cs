using System;
using VladislavTsurikov.ActionFlow.Runtime;

namespace VladislavTsurikov.EntityDataAction.Runtime.Core
{
    public class EntityActionCollection : ActionCollection
    {
        [NonSerialized]
        private Entity _entity;

        public Entity Entity
        {
            get => _entity;
            internal set => _entity = value;
        }
    }
}
