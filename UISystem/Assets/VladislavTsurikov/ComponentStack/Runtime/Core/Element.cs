using System;
using System.Linq;
using VladislavTsurikov.AttributeUtility.Runtime;
using VladislavTsurikov.ReflectionUtility;

namespace VladislavTsurikov.ComponentStack.Runtime.Core
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

        protected virtual void FirstSetupComponent(object[] setupData = null)
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
                FirstSetupComponent(setupData);
            }

            SetupComponent(setupData);
            IsSetup = true;
        }

        internal void OnReset(Element oldElement) => OnResetElement(oldElement);
    }
}
