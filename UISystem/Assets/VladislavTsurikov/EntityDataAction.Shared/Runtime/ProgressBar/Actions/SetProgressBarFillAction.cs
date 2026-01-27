using System.Threading;
using Cysharp.Threading.Tasks;
using OdinSerializer;
using UnityEngine;
using VladislavTsurikov.EntityDataAction.Runtime;
using VladislavTsurikov.EntityDataAction.Runtime.Core;
using VladislavTsurikov.ReflectionUtility;

namespace VladislavTsurikov.EntityDataAction.Shared.Runtime.ProgressBar
{
    [RunOnDirtyData(typeof(ProgressBarData))]
    [RequiresData(typeof(ProgressBarData))]
    [Name("UI/Common/SetProgressBarFillAction")]
    public sealed class SetProgressBarFillAction : EntityAction
    {
        [OdinSerialize]
        private RectTransform _fill;

        protected override UniTask<bool> Run(CancellationToken token)
        {
            ProgressBarData data = Get<ProgressBarData>();
            float progress = Mathf.Clamp01(data.Progress01);

            if (_fill != null)
            {
                _fill.anchorMax = new Vector2(progress, _fill.anchorMax.y);
                _fill.sizeDelta = Vector2.zero;
            }

            return UniTask.FromResult(true);
        }
    }
}
