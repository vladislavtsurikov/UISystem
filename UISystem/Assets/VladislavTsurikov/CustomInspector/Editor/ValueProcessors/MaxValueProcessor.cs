using System;
using System.Reflection;
using VladislavTsurikov.CustomInspector.Runtime;

namespace VladislavTsurikov.CustomInspector.Editor.Core
{
    public sealed class MaxValueProcessorMatcher : FieldValueProcessorMatcher
    {
        public override bool CanProcess(Attribute attribute) => attribute is MaxAttribute;
        public override Type ProcessorType => typeof(MaxValueProcessor);
    }

    public sealed class MaxValueProcessor : FieldValueProcessor
    {
        private float _max;

        public override void Initialize(Attribute attribute)
        {
            base.Initialize(attribute);
            _max = ((MaxAttribute)attribute).Value;
        }

        public override object Process(FieldInfo field, object target, object value)
        {
            if (value == null)
            {
                return null;
            }

            if (value is int intValue)
            {
                return Math.Min((int)_max, intValue);
            }

            if (value is float floatValue)
            {
                return Math.Min(_max, floatValue);
            }

            if (value is double doubleValue)
            {
                return Math.Min((double)_max, doubleValue);
            }

            if (value is long longValue)
            {
                return Math.Min((long)_max, longValue);
            }

            if (value is short shortValue)
            {
                return (short)Math.Min((short)_max, shortValue);
            }

            if (value is byte byteValue)
            {
                var maxValue = _max <= 0f ? (byte)0 : (byte)_max;
                return (byte)Math.Min(maxValue, byteValue);
            }

            if (value is sbyte sbyteValue)
            {
                return (sbyte)Math.Min((sbyte)_max, sbyteValue);
            }

            if (value is uint uintValue)
            {
                var maxValue = _max <= 0f ? 0u : (uint)_max;
                return uintValue > maxValue ? maxValue : uintValue;
            }

            if (value is ulong ulongValue)
            {
                var maxValue = _max <= 0f ? 0ul : (ulong)_max;
                return ulongValue > maxValue ? maxValue : ulongValue;
            }

            if (value is ushort ushortValue)
            {
                var maxValue = _max <= 0f ? (ushort)0 : (ushort)_max;
                return (ushort)Math.Min(maxValue, ushortValue);
            }

            if (value is decimal decimalValue)
            {
                return Math.Min((decimal)_max, decimalValue);
            }

            return value;
        }
    }
}
