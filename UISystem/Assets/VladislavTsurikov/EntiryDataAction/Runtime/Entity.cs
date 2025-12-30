using System;
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

        protected virtual void OnEnable()
        {
            DirtyRunner ??= new DirtyActionRunner(this, _data, _actions);
            DirtyRunner.Setup();

            _actions ??= new EntityActionCollection();
            _actions.Entity = this;

            if (Active)
            {
                _data.Setup(false);
                _actions.Setup(false);

                _data.ElementAdded += HandleDataChanged;
                _data.ElementRemoved += HandleDataChanged;
            }

            CreateDefaultData();
            CreateDefaultActions();
            RefreshActions();
        }

        protected virtual void OnDisable()
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

        protected virtual Type[] ComponentDataTypesToCreate()
        {
            return Array.Empty<Type>();
        }

        protected virtual Type[] ActionTypesToCreate()
        {
            return Array.Empty<Type>();
        }

        private void CreateDefaultData()
        {
            Type[] types = ComponentDataTypesToCreate();
            if (types == null || types.Length == 0)
            {
                return;
            }

            _data.CreateIfMissingType(types);
        }

        private void CreateDefaultActions()
        {
            if (_actions == null)
            {
                return;
            }

            Type[] types = ActionTypesToCreate();
            if (types == null || types.Length == 0)
            {
                return;
            }

            _actions.CreateComponentsIfMissingType(types);
        }
    }
}
