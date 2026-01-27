using System.Threading;
using Cysharp.Threading.Tasks;
using VladislavTsurikov.EntityDataAction.Runtime;
using VladislavTsurikov.EntityDataAction.Runtime.Core;

namespace VladislavTsurikov.EntityDataAction.Shared.Runtime.Refresh
{
    [RunOnDirtyData(typeof(RefreshData))]
    [RequiresData(typeof(RefreshData))]
    public abstract class RefreshAction : EntityAction
    {
        protected override UniTask<bool> Run(CancellationToken token)
        {
            RefreshData data = Get<RefreshData>();

            if (!data.Refresh)
            {
                return UniTask.FromResult(true);
            }

            OnRefresh(token);

            data.Refresh = false;

            return UniTask.FromResult(true);
        }

        protected abstract void OnRefresh(CancellationToken token);
    }
}
