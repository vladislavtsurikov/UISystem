using System.Globalization;

namespace VladislavTsurikov.MegaWorld.Runtime.Common.Settings.FilterSettings.MaskFilterSystem.Noise.API
{
    /// <summary>
    ///     Enum for determining the type of a given HLSL value during shader generation
    /// </summary>
    public enum HlslValueType
    {
        Float = 0,
        Float2,
        Float3,
        Float4
    }

    /// <summary>
    ///     Representation for an HLSL float
    /// </summary>
    public struct HlslFloat
    {
        /// <summary>
        ///     The value for the HLSL float
        /// </summary>
        public float Val;

        /// <summary>
        ///     The constructor for an HlslFloat
        /// </summary>
        /// <param name="val"> The GPU value for this HlslFloat </param>
        public HlslFloat(float val) => Val = val;
    }

    /// <summary>
    // Representation for an HLSL float2
    /// </summary>
    public struct HlslFloat2
    {
        /// <summary>
        ///     The x-compenent for the HLSL float2
        /// </summary>
        public float X;

        /// <summary>
        ///     The y-compenent for the HLSL float2
        /// </summary>
        public float Y;

        /// <summary>
        ///     The constructor for an HlslFloat2
        /// </summary>
        /// <param name="x"> The GPU value of the x-component to be used for this HlslFloat2 </param>
        /// <param name="y"> The GPU value of the y-component to be used for this HlslFloat2 </param>
        public HlslFloat2(float x, float y)
        {
            X = x;
            Y = y;
        }
    }

    /// <summary>
    ///     Representation for an HLSL float3
    /// </summary>
    public struct HlslFloat3
    {
        /// <summary>
        ///     The x-compenent for the HLSL float3
        /// </summary>
        public float X;

        /// <summary>
        ///     The y-compenent for the HLSL float3
        /// </summary>
        public float Y;

        /// <summary>
        ///     The z-compenent for the HLSL float3
        /// </summary>
        public float Z;

        /// <summary>
        ///     The constructor for an HlslFloat3
        /// </summary>
        /// <param name="x"> The GPU value of the x-component to be used for this HlslFloat3 </param>
        /// <param name="y"> The GPU value of the y-component to be used for this HlslFloat3 </param>
        /// <param name="z"> The GPU value of the z-component to be used for this HlslFloat3 </param>
        public HlslFloat3(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }
    }

    /// <summary>
    ///     Representation for an HLSL float4
    /// </summary>
    public struct HlslFloat4
    {
        /// <summary>
        ///     The x-compenent for the HLSL float4
        /// </summary>
        public float X;

        /// <summary>
        ///     The y-compenent for the HLSL float4
        /// </summary>
        public float Y;

        /// <summary>
        ///     The z-compenent for the HLSL float4
        /// </summary>
        public float Z;

        /// <summary>
        ///     The w-compenent for the HLSL float4
        /// </summary>
        public float W;

        /// <summary>
        ///     The constructor for an HlslFloat4
        /// </summary>
        /// <param name="x"> The GPU value of the x-component to be used for this HlslFloat4 </param>
        /// <param name="y"> The GPU value of the y-component to be used for this HlslFloat4 </param>
        /// <param name="z"> The GPU value of the z-component to be used for this HlslFloat4 </param>
        /// <param name="w"> The GPU value of the w-component to be used for this HlslFloat4 </param>
        public HlslFloat4(float x, float y, float z, float w)
        {
            X = x;
            Y = y;
            Z = z;
            W = w;
        }
    }

    /// <summary>
    ///     Representation for an HLSL function input parameter
    /// </summary>
    public struct HlslInput
    {
        /// <summary>
        ///     The name of this HLSL function input parameter
        /// </summary>
        public string Name;

        /// <summary>
        ///     Returns the value type for the variable this HlslInput represents
        /// </summary>
        public HlslValueType valueType { get; private set; }

        private HlslFloat _mFloatValue;

        /// <summary>
        ///     Returns the HLSL float value. Sets the HLSL float value and value
        ///     type to HlslValueType.Float
        /// </summary>
        public HlslFloat floatValue
        {
            get => _mFloatValue;
            set
            {
                valueType = HlslValueType.Float;
                _mFloatValue = value;
            }
        }

        private HlslFloat2 _mFloat2Value;

        /// <summary>
        ///     Returns the HLSL float2 value. Sets the HLSL float value and value
        ///     type to HlslValueType.Float2
        /// </summary>
        public HlslFloat2 float2Value
        {
            get => _mFloat2Value;
            set
            {
                valueType = HlslValueType.Float2;
                _mFloat2Value = value;
            }
        }

        private HlslFloat3 _mFloat3Value;

        /// <summary>
        ///     Returns the HLSL float3 value. Sets the HLSL float value and value
        ///     type to HlslValueType.Float3
        /// </summary>
        public HlslFloat3 float3Value
        {
            get => _mFloat3Value;
            set
            {
                valueType = HlslValueType.Float3;
                _mFloat3Value = value;
            }
        }

        private HlslFloat4 _mFloat4Value;

        /// <summary>
        ///     Returns the HLSL float4 value. Sets the HLSL float value and value
        ///     type to HlslValueType.Float4
        /// </summary>
        public HlslFloat4 float4Value
        {
            get => _mFloat4Value;
            set
            {
                valueType = HlslValueType.Float4;
                _mFloat4Value = value;
            }
        }

        /// <summary>
        ///     Returns the string representation of the HlslValueType for this struct
        /// </summary>
        public string GetHlslValueTypeString()
        {
            switch (valueType)
            {
                case HlslValueType.Float:
                    return "float";
                case HlslValueType.Float2:
                    return "float2";
                case HlslValueType.Float3:
                    return "float3";
                case HlslValueType.Float4:
                    return "float4";
            }

            return "unsupported_type";
        }


        /// <summary>
        ///     Returns the formatted HLSL string for the default value declaration for this struct's HlslValueType
        /// </summary>
        public string GetDefaultValueString()
        {
            var valueTypeString = GetHlslValueTypeString();
            string constructedValueString = null;

            switch (valueType)
            {
                case HlslValueType.Float:
                    constructedValueString = floatValue.Val.ToString(CultureInfo.InvariantCulture);
                    break;
                case HlslValueType.Float2:
                    constructedValueString = string.Format(CultureInfo.InvariantCulture,
                        "{0}({1})", valueTypeString,
                        float2Value.X.ToString(CultureInfo.InvariantCulture) + ", " +
                        float2Value.Y.ToString(CultureInfo.InvariantCulture));
                    break;
                case HlslValueType.Float3:
                    constructedValueString = string.Format(CultureInfo.InvariantCulture,
                        "{0}({1})", valueTypeString,
                        float3Value.X.ToString(CultureInfo.InvariantCulture) + ", " +
                        float3Value.Y.ToString(CultureInfo.InvariantCulture) + ", " +
                        float3Value.Z.ToString(CultureInfo.InvariantCulture));
                    break;
                case HlslValueType.Float4:
                    constructedValueString = string.Format(CultureInfo.InvariantCulture,
                        "{0}({1})", valueTypeString,
                        float4Value.X.ToString(CultureInfo.InvariantCulture) + ", " +
                        float4Value.Y.ToString(CultureInfo.InvariantCulture) + ", " +
                        float4Value.Z.ToString(CultureInfo.InvariantCulture) + ", " +
                        float4Value.W.ToString(CultureInfo.InvariantCulture));
                    break;
                default:
                    return "unsupported_type()";
            }

            return constructedValueString;
        }
    }

    // public struct HlslOutput
    // {
    //     public string name;
    //     public HlslValueType valueType;

    //     public HlslOutput(string name, HlslValueType valueType)
    //     {
    //         this.name = name;
    //         this.valueType = valueType;
    //     }
    // }

    // public struct HlslStructDescriptor
    // {
    //     public List<HlslInput> members;
    // }

    // public struct HlslFunctionDescriptor
    // {
    //     public List<HlslInput> inputs;
    //     public List<HlslOutput> outputs;
    // }
}
