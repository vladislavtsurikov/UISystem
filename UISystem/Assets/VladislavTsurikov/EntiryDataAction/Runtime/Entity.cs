using System;
using Cysharp.Threading.Tasks;
using OdinSerializer;
using Plugins.VladislavTsurikov.EntiryDataAction.Runtime;
using UnityEngine;
using Action = VladislavTsurikov.ActionFlow.Runtime.Actions.Action;

namespace VladislavTsurikov.EntityDataActionFramework
{
    [ExecuteInEditMode]
    public class Entity : SerializedMonoBehaviour
    {
        [OdinSerialize]
        private EntityDataCollection _data = new EntityDataCollection();
        [OdinSerialize]
        private EntityActionCollection _actions;
        [NonSerialized]
        private bool _active;

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

        protected virtual void OnEnable()
        {
            _data ??= new EntityDataCollection();
            _data.Entity = this;
            _actions ??= new EntityActionCollection();
            _actions.Entity = this;

            DirtyRunner ??= new DirtyActionRunner(this, _data, _actions);
            DirtyRunner.Setup();

            CreateDefaultData();
            CreateDefaultActions();
            RefreshActions();

            if (Active)
            {
                _data.Setup();
                _actions.Setup();

                _data.ElementAdded += HandleDataChanged;
                _data.ElementRemoved += HandleDataChanged;

                _actions.Run().Forget();

                //DirtyRunner?.TriggerAll();
            }
        }

        protected virtual void OnDisable()
        {
            _data.ElementAdded -= HandleDataChanged;
            _data.ElementRemoved -= HandleDataChanged;

            _data.OnDisable();
            _actions.OnDisable();

            DirtyRunner?.OnDisable();
        }

        private void HandleDataChanged(int index)
        {
            RefreshActions();

            DirtyRunner?.TriggerAll();
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

        protected virtual Type[] ComponentDataTypesToCreate()
        {
            return null;
        }

        protected virtual Type[] ActionTypesToCreate()
        {
            return null;
        }

        private void CreateDefaultData()
        {
            Type[] types = ComponentDataTypesToCreate();
            if (types == null)
            {
                return;
            }

            _data.SyncToTypes(types);
        }

        private void CreateDefaultActions()
        {
            if (_actions == null)
            {
                return;
            }

            Type[] types = ActionTypesToCreate();
            if (types == null)
            {
                return;
            }

            _actions.SyncToTypes(types);
        }
    }
}
