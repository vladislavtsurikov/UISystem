using OdinSerializer;
using Plugins.VladislavTsurikov.EntiryDataAction.Runtime;
using UnityEngine;
using VladislavTsurikov.ActionFlow.Runtime.Actions;

namespace VladislavTsurikov.EntityDataActionFramework
{
    [ExecuteInEditMode]
    public sealed class Entity : SerializedMonoBehaviour
    {
        [OdinSerialize]
        private EntityDataCollection _data = new EntityDataCollection();
        private bool _active;

        [OdinSerialize]
        private EntityActionCollection _actions;

        internal DirtyActionRunner DirtyRunner;

        public EntityDataCollection Data => _data;
        public EntityActionCollection Actions => _actions;

        internal bool Active
        {
            get
            {
                if (Application.isPlaying)
                {
                    return true;
                }

                return _active;
            }
            set
            {
                if (Application.isPlaying)
                {
                    return;
                }

                if (_active == value)
                {
                    return;
                }

                _active = value;

                if (!isActiveAndEnabled)
                {
                    return;
                }

                if (value)
                {
                    OnDisable();
                    OnEnable();
                }
                else
                {
                    OnDisable();
                }
            }
        }

        private void OnEnable()
        {
            _actions ??= new EntityActionCollection();
            _data ??= new EntityDataCollection();
            _actions.Entity = this;
            _data.Entity = this;

            DirtyRunner ??= new DirtyActionRunner(this, _data, _actions);
            DirtyRunner.Setup();

            RefreshActions();

            if (Active)
            {
                _data.Setup(false);
                _actions.Setup(false);

                _data.ElementAdded += HandleDataChanged;
                _data.ElementRemoved += HandleDataChanged;

                DirtyRunner.TriggerAll();
            }
        }

        private void OnDisable()
        {
            _data.ElementAdded -= HandleDataChanged;
            _data.ElementRemoved -= HandleDataChanged;

            if (DirtyRunner != null)
            {
                DirtyRunner.OnDisable();
                DirtyRunner = null;
            }
        }

        private void HandleDataChanged(int index)
        {
            RefreshActions();

            if (DirtyRunner != null)
            {
                DirtyRunner.TriggerAll();
            }
        }

        private void RefreshActions()
        {
            for (int i = 0; i < _actions.ElementList.Count; i++)
            {
                Action action = _actions.ElementList[i];
                bool isAvailable = RequiresDataUtility.IsRequirementsMet(_data, action.GetType());
                action.Active = isAvailable;
            }
        }
    }
}
