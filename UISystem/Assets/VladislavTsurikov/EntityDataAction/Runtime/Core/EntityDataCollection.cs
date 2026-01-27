using System;
using VladislavTsurikov.Nody.Runtime.AdvancedNodeStack;

namespace VladislavTsurikov.EntityDataAction.Runtime.Core
{
    public sealed class EntityDataCollection : NodeStackOnlyDifferentTypes<ComponentData>
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
