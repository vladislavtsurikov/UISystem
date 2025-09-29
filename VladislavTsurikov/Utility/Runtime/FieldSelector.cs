using System;
using System.Reflection;
using OdinSerializer;

namespace QuestsSystem.IntegrationActionFlow.Pointer
{
    [Serializable]
    public class FieldSelector
    {
        [OdinSerialize]
        private Type _declaringType;

        [OdinSerialize]
        private string _fieldName;

        public FieldInfo FieldInfo => !string.IsNullOrEmpty(_fieldName) && _declaringType != null
            ? _declaringType.GetField(_fieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
            : null;

        public Type DeclaringType
        {
            get => _declaringType;
            set
            {
                _declaringType = value;
                _fieldName = null;
            }
        }

        public string FieldName
        {
            get => _fieldName;
            set
            {
                if (!string.IsNullOrEmpty(value) && _declaringType != null)
                {
                    FieldInfo field = _declaringType.GetField(value,
                        BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                    if (field == null || !IsValidFieldType(field.FieldType))
                    {
                        throw new InvalidOperationException(
                            $"Field '{value}' is not a valid type in '{_declaringType}'.");
                    }
                }

                _fieldName = value;
            }
        }

        public virtual bool IsValidFieldType(Type fieldType) => true;

        public object GetFieldValue(object target)
        {
            if (FieldInfo == null || target == null)
            {
                return null;
            }

            return FieldInfo.GetValue(target);
        }
    }
}
