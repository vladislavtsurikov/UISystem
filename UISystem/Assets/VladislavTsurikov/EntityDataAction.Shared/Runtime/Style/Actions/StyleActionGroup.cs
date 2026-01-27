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
    [Name("UI/Common/StyleGroup")]
    public sealed class StyleActionGroup : EntityAction
    {
        [OdinSerialize]
        private string _styleName = "Normal";

        [OdinSerialize]
        [HideInInspector]
        private StyleActionCollection _actions = new StyleActionCollection();

        public string StyleName => _styleName;

        public StyleActionCollection Actions => _actions;

        public override string Name =>
            string.IsNullOrWhiteSpace(_styleName) ? base.Name : $"{base.Name} ({_styleName})";

        protected override void SetupComponent(object[] setupData = null)
        {
            EnsureActions(setupData);
        }

        protected override void OnDisableElement()
        {
            if (_actions != null)
            {
                _actions.OnDisable();
            }
        }

        protected override UniTask<bool> Run(CancellationToken token)
        {
            if (_actions == null)
            {
                Debug.LogWarning($"{GetType().Name}: _actions is null");
                return UniTask.FromResult(true);
            }

            if (!IsStyleActive())
            {
                return UniTask.FromResult(true);
            }

            return _actions.Run(token);
        }

        private void EnsureActions(object[] setupData)
        {
            if (_actions == null)
            {
                _actions = new StyleActionCollection();
            }

            _actions.Entity = Entity;
            _actions.Setup(true, setupData);
        }

        private bool IsStyleActive()
        {
            if (string.IsNullOrWhiteSpace(_styleName))
            {
                return false;
            }

            StyleStateData styleData = Get<StyleStateData>();
            return styleData != null && styleData.HasStyle(_styleName);
        }
    }
}
