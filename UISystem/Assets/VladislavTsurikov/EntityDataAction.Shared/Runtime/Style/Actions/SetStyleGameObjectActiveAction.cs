using System.Threading;
using Cysharp.Threading.Tasks;
using OdinSerializer;
using UnityEngine;
using VladislavTsurikov.EntityDataAction.Runtime;
using VladislavTsurikov.EntityDataAction.Runtime.Core;
using VladislavTsurikov.ReflectionUtility;

namespace VladislavTsurikov.EntityDataAction.Shared.Runtime.Style
{
    [RunOnDirtyData(typeof(StyleStateData))]
    [RequiresData(typeof(StyleStateData))]
    [Name("UI/Common/Style/SetStyleGameObjectActiveAction")]
    public sealed class SetStyleGameObjectActiveAction : EntityAction
    {
        [OdinSerialize]
        private GameObject _target;

        [OdinSerialize]
        private bool _activeState = true;

        protected override UniTask<bool> Run(CancellationToken token)
        {
            _target.SetActive(_activeState);

            return UniTask.FromResult(true);
        }
    }
}
