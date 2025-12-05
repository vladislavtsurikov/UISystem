using System;
using OdinSerializer;
using UnityEngine;

namespace VladislavTsurikov.ActionFlow.Runtime.Variables
{
    [Serializable]
    public abstract class Variable<T> : SerializedScriptableObject
    {
        [SerializeField]
        private string _uniqueID;

        [SerializeField]
        protected T _value;

        public string UniqueID => _uniqueID;

        public virtual T Value
        {
            get => Get();
            set => Set(value);
        }

        private void OnEnable()
        {
            if (string.IsNullOrEmpty(_uniqueID))
            {
                _uniqueID = Guid.NewGuid().ToString();
            }
        }

        public event Action<T> OnValueChanged;

        protected virtual T Get() => _value;

        protected virtual void Set(T value)
        {
            if (!Equals(_value, value))
            {
                _value = value;
                OnValueChanged?.Invoke(_value);
            }
        }

        public override string ToString() => _value != null ? _value.ToString() : "null";
    }
}
