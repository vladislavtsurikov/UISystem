#if ENTITY_DATA_ACTION_MPUIKIT
using System.Threading;
using Cysharp.Threading.Tasks;
using MPUIKIT;
using OdinSerializer;
using UnityEngine;
using VladislavTsurikov.EntityDataAction.Runtime;
using VladislavTsurikov.EntityDataAction.Runtime.Core;
using VladislavTsurikov.ReflectionUtility;

namespace VladislavTsurikov.EntityDataAction.Shared.Runtime.Style
{
    [RunOnDirtyData(typeof(StyleStateData))]
    [RequiresData(typeof(StyleStateData))]
    [Name("UI/Common/Style/SetStyleMPImageOutlineAction")]
    public sealed class SetStyleMPImageOutlineAction : EntityAction
    {
        [OdinSerialize]
        private MPImage _target;

        [OdinSerialize]
        private float _activeOutlineWidth = 2f;

        [OdinSerialize]
        private Color _activeOutlineColor = Color.white;

        protected override UniTask<bool> Run(CancellationToken token)
        {
            _target.OutlineTop = _activeOutlineWidth;
            _target.OutlineBottom = _activeOutlineWidth;
            _target.OutlineLeft = _activeOutlineWidth;
            _target.OutlineRight = _activeOutlineWidth;
            _target.OutlineColor = _activeOutlineColor;

            return UniTask.FromResult(true);
        }
    }
}
#endif
