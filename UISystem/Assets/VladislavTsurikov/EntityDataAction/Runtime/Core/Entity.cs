using System;
using Cysharp.Threading.Tasks;
using OdinSerializer;
using UnityEngine;

namespace VladislavTsurikov.EntityDataAction.Runtime.Core
{
    [ExecuteInEditMode]
    public class Entity : SerializedMonoBehaviour
    {
        [OdinSerialize]
        private EntityDataCollection _data;
        [OdinSerialize]
        private EntityActionCollection _actions;
        [OdinSerialize]
        private bool _localActive = true;

        internal DirtyActionRunner DirtyRunner;

        public EntityDataCollection Data => _data;
        public EntityActionCollection Actions => _actions;

        public bool IsSetup { get; private set; }

        public bool LocalActive
        {
            get => _localActive;
            set
            {
                if (_localActive == value)
                {
                    return;
                }

                _localActive = value;
                HandleActiveChanged();
            }
        }

        public bool GlobalActive
        {
            get => EntityDataActionGlobalSettings.Active;
            set => EntityDataActionGlobalSettings.Active = value;
        }

        internal bool Active
        {
            get
            {
#if UNITY_EDITOR
                if (UnityEditor.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage() != null)
                {
                    return false;
                }
#endif

                return _localActive && EntityDataActionGlobalSettings.Active;
            }
            set
            {
                LocalActive = value;
            }
        }

        protected virtual void OnSetupEntity()
        {
            if (Active)
            {
                _data.Setup();
                _actions.Setup();

                _data.ElementAdded += HandleDataChanged;
                _data.ElementRemoved += HandleDataChanged;

                _actions.Run().Forget();
            }
        }

        protected void OnEnable()
        {
            Setup();
        }

        internal void Setup()
        {
            EntityDataActionGlobalSettings.ActiveChanged -= HandleActiveChanged;
            EntityDataActionGlobalSettings.ActiveChanged += HandleActiveChanged;

            _data ??= new EntityDataCollection();
            _data.Entity = this;
            _actions ??= new EntityActionCollection();
            _actions.Entity = this;

            DirtyRunner ??= new DirtyActionRunner(this, _data, _actions);
            DirtyRunner.Setup();

            CreateDefaultData();
            CreateDefaultActions();

            OnAfterCreateDataAndActions();

            OnSetupEntity();

            IsSetup = true;
        }

        protected void OnDisable()
        {
            EntityDataActionGlobalSettings.ActiveChanged -= HandleActiveChanged;
            _data.ElementAdded -= HandleDataChanged;
            _data.ElementRemoved -= HandleDataChanged;

            _data.OnDisable();
            _actions.OnDisable();

            DirtyRunner?.OnDisable();

            IsSetup = false;
        }

        private void HandleActiveChanged()
        {
            if (!isActiveAndEnabled)
            {
                return;
            }

#if UNITY_EDITOR
            if (UnityEditor.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage() != null)
            {
                OnDisable();
                return;
            }
#endif

            if (_localActive && EntityDataActionGlobalSettings.Active)
            {
                OnDisable();
                OnEnable();
            }
            else
            {
                OnDisable();
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

        protected virtual void OnAfterCreateDataAndActions()
        {
        }

        public T GetData<T>() where T : ComponentData
        {
            return Data.GetElement<T>();
        }

        public T GetAction<T>() where T : EntityAction
        {
            return Actions.GetElement<T>();
        }

        protected void HandleDataChanged(int index)
        {
            DirtyRunner?.TriggerAll();
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
