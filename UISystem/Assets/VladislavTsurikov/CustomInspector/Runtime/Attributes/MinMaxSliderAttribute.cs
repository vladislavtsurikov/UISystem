using System;

namespace VladislavTsurikov.CustomInspector.Runtime
{
    public enum MinMaxSliderLabelPreset
    {
        None,
        ScaleZeroToFive,
        SlopeDegrees
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class MinMaxSliderAttribute : Attribute
    {
        public MinMaxSliderAttribute(float min, float max)
        {
            Min = min;
            Max = max;
        }

        public MinMaxSliderAttribute(float min, float max, string maxFieldName)
            : this(min, max)
        {
            MaxFieldName = maxFieldName;
        }

        public float Min { get; }
        public float Max { get; }
        public string MaxFieldName { get; }
        public string MaxValueMemberName { get; set; }
        public string UniformToggleFieldName { get; set; }
        public string LabelOverride { get; set; }
        public MinMaxSliderLabelPreset LabelPreset { get; set; } = MinMaxSliderLabelPreset.None;
        public string LabelLeft { get; set; }
        public string LabelCenter { get; set; }
        public string LabelRight { get; set; }
        public bool ShowFields { get; set; } = true;
    }
}
