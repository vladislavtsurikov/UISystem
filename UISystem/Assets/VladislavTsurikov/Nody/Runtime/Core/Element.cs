using System;
using System.Linq;
using OdinSerializer;
using UnityEngine;
using VladislavTsurikov.AttributeUtility.Runtime;
using VladislavTsurikov.ReflectionUtility;

namespace VladislavTsurikov.Nody.Runtime.Core
{
    [Serializable]
    public class Element : IHasName, IDisableable
    {
        [HideInInspector]
        public bool SelectSettingsFoldout = true;
        private object[] _setupData;

        [NonSerialized]
        [HideInInspector]
        public bool Renaming;

        [NonSerialized]
        [HideInInspector]
        public string RenamingName;

        [field: NonSerialized]
        public bool IsSetup { get; protected set; }

        [field: NonSerialized]
        public bool IsHappenedReset { get; internal set; }

        [OdinSerialize]
        [HideInInspector]
        protected bool _active = true;

        [NonSerialized]
        private bool _isDirty;

        public bool IsDirty => _isDirty;

        void IDisableable.OnDisable()
        {
            IsSetup = false;
            OnDisableElement();
        }

        public virtual string Name
        {
            get
            {
                NameAttribute nameAttribute = GetType().GetAttribute<NameAttribute>();

                if (nameAttribute != null)
                {
                    return nameAttribute.Name.Split('/').Last();
                }

                return GetType().ToString().Split('.').Last();
            }
            set { }
        }

        protected virtual void SetupComponent(object[] setupData = null)
        {
        }

        protected virtual void OnFirstSetupComponent(object[] setupData = null)
        {
        }

        protected virtual void OnDisableElement()
        {
        }

        protected virtual void OnResetElement(Element oldElement)
        {
        }

        protected virtual void OnChangeActive()
        {
            if (Active)
            {
                SetupWithSetupData(false, _setupData);
                return;
            }

            if (IsSetup)
            {
                IsSetup = false;
                OnDisableElement();
            }
        }

        public virtual bool ShowActiveToggle() => true;

        public virtual bool Active
        {
            get => _active;
            set
            {
                if (_active != value)
                {
                    _active = value;
                    OnChangeActive();
                }
            }
        }

        public void Setup(bool force = false) => SetupWithSetupData(force, _setupData);

        public void SetupWithSetupData(bool force = false, object[] setupData = null)
        {
            if (!force)
            {
                if (!Active && IsSetup)
                {
                    return;
                }
            }

            _setupData = setupData;

            IsSetup = false;
            OnDisableElement();

            if (!Active)
            {
                return;
            }

            try
            {
                if (!IsSetup)
                {
                    OnFirstSetupComponent(setupData);
                }

                SetupComponent(setupData);
                IsSetup = true;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[{GetType().Name}] SetupWithSetupData failed: {ex.Message}\nStackTrace: {ex.StackTrace}");
            }
        }

        internal void OnReset(Element oldElement) => OnResetElement(oldElement);

        public void MarkDirty()
        {
            _isDirty = true;
            OnDirtied();
        }

        internal void ClearDirtyInternal()
        {
            _isDirty = false;
        }

        protected virtual void OnDirtied()
        {
        }
    }
}
