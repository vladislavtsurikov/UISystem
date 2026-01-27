using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using OdinSerializer;
using VladislavTsurikov.EntityDataAction.Runtime;
using VladislavTsurikov.EntityDataAction.Runtime.Core;
using VladislavTsurikov.ReflectionUtility;

namespace VladislavTsurikov.EntityDataAction.Shared.Runtime.Refresh
{
    [Name("UI/Common/Refresh/RefreshEntitiesAction")]
    public sealed class RefreshEntitiesAction : RefreshAction
    {
        [OdinSerialize]
        private List<Entity> _entitiesToRefresh = new List<Entity>();

        protected override void OnRefresh(CancellationToken token)
        {
            foreach (Entity entity in _entitiesToRefresh)
            {
                if (entity != null && entity.isActiveAndEnabled)
                {
                    entity.Actions.Run(token).Forget();
                }
            }
        }
    }
}
