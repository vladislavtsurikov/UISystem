using System.Threading;
using Cysharp.Threading.Tasks;
using OdinSerializer;
using UnityEngine;
using UnityEngine.UI;
using VladislavTsurikov.EntityDataAction.Runtime;
using VladislavTsurikov.EntityDataAction.Runtime.Core;
using VladislavTsurikov.ReflectionUtility;

namespace VladislavTsurikov.EntityDataAction.Shared.Runtime.Style
{
    [RunOnDirtyData(typeof(StyleStateData))]
    [RequiresData(typeof(StyleStateData))]
    [Name("UI/Common/Style/SetStyleGraphicColorAction")]
    public sealed class SetStyleGraphicColorAction : EntityAction
    {
        [OdinSerialize]
        private Graphic _target;

        [OdinSerialize]
        private Color _activeColor = Color.white;

        protected override UniTask<bool> Run(CancellationToken token)
        {
            _target.color = _activeColor;

            return UniTask.FromResult(true);
        }
    }
}
