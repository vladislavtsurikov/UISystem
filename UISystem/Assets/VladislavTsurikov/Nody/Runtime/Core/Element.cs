using System;
using System.Linq;
using VladislavTsurikov.AttributeUtility.Runtime;
using VladislavTsurikov.ReflectionUtility;

namespace VladislavTsurikov.Nody.Runtime.Core
{
    [Serializable]
    public class Element : IHasName, IDisableable
    {
        public bool SelectSettingsFoldout = true;
        private object[] _setupData;

        [NonSerialized]
        public bool Renaming;

        [NonSerialized]
        public string RenamingName;

        [field: NonSerialized]
        public bool IsSetup { get; protected set; }

        [field: NonSerialized]
        public bool IsHappenedReset { get; internal set; }

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

        public virtual bool ShowActiveToggle() => true;

        public void Setup(bool force = false) => SetupWithSetupData(force, _setupData);

        public void SetupWithSetupData(bool force = false, object[] setupData = null)
        {
            if (!force && IsSetup)
            {
                return;
            }

            _setupData = setupData;

            IsSetup = false;
            OnDisableElement();

            if (!IsSetup)
            {
                OnFirstSetupComponent(setupData);
            }

            SetupComponent(setupData);
            IsSetup = true;
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
