using System;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.FilterSettings.MaskFilterSystem.Noise.API;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.FilterSettings.MaskFilterSystem.Shaders;

namespace VladislavTsurikov.MegaWorld.Runtime.Common.Settings.FilterSettings.MaskFilterSystem.Noise.NoiseTypes
{
    /// <summary>
    ///     A NoiseType implementation for Value noise
    /// </summary>
    [Serializable]
    public class ValueNoise : NoiseType<ValueNoise>
    {
        private static ValueNoise _sInstance;

        private static readonly NoiseTypeDescriptor _desc = new()
        {
            Name = "Value",
            OutputDir = MaskFilterShadersPath.Path + "/NoiseLib/",
            SourcePath = MaskFilterShadersPath.Path + "/NoiseLib/Implementation/ValueImpl.hlsl",
            SupportedDimensions = NoiseDimensionFlags._1D | NoiseDimensionFlags._2D | NoiseDimensionFlags._3D,
            InputStructDefinition = null
        };

        public static ValueNoise Instance
        {
            get
            {
                if (_sInstance == null)
                {
                    _sInstance = new ValueNoise();
                }

                return _sInstance;
            }
        }

        public override NoiseTypeDescriptor GetDescription() => _desc;
    }
}
