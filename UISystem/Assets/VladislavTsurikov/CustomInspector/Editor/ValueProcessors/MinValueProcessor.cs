using System;
using System.Reflection;
using VladislavTsurikov.CustomInspector.Runtime;

namespace VladislavTsurikov.CustomInspector.Editor.Core
{
    public sealed class MinValueProcessorMatcher : FieldValueProcessorMatcher
    {
        public override bool CanProcess(Attribute attribute) => attribute is MinAttribute;
        public override Type ProcessorType => typeof(MinValueProcessor);
    }

    public sealed class MinValueProcessor : FieldValueProcessor
    {
        private float _min;

        public override void Initialize(Attribute attribute)
        {
            base.Initialize(attribute);
            _min = ((MinAttribute)attribute).Value;
        }

        public override object Process(FieldInfo field, object target, object value)
        {
            if (value == null)
            {
                return null;
            }

            if (value is int intValue)
            {
                return Math.Max((int)_min, intValue);
            }

            if (value is float floatValue)
            {
                return Math.Max(_min, floatValue);
            }

            if (value is double doubleValue)
            {
                return Math.Max((double)_min, doubleValue);
            }

            if (value is long longValue)
            {
                return Math.Max((long)_min, longValue);
            }

            if (value is short shortValue)
            {
                return (short)Math.Max((short)_min, shortValue);
            }

            if (value is byte byteValue)
            {
                var minValue = _min <= 0f ? (byte)0 : (byte)_min;
                return (byte)Math.Max(minValue, byteValue);
            }

            if (value is sbyte sbyteValue)
            {
                return (sbyte)Math.Max((sbyte)_min, sbyteValue);
            }

            if (value is uint uintValue)
            {
                var minValue = _min <= 0f ? 0u : (uint)_min;
                return uintValue < minValue ? minValue : uintValue;
            }

            if (value is ulong ulongValue)
            {
                var minValue = _min <= 0f ? 0ul : (ulong)_min;
                return ulongValue < minValue ? minValue : ulongValue;
            }

            if (value is ushort ushortValue)
            {
                var minValue = _min <= 0f ? (ushort)0 : (ushort)_min;
                return (ushort)Math.Max(minValue, ushortValue);
            }

            if (value is decimal decimalValue)
            {
                return Math.Max((decimal)_min, decimalValue);
            }

            return value;
        }
    }
}
