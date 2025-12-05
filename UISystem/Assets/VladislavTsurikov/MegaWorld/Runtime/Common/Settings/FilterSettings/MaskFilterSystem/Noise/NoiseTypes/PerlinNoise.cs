using System;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.FilterSettings.MaskFilterSystem.Noise.API;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.FilterSettings.MaskFilterSystem.Shaders;

namespace VladislavTsurikov.MegaWorld.Runtime.Common.Settings.FilterSettings.MaskFilterSystem.Noise.NoiseTypes
{
    /// <summary>
    ///     A NoiseType implementation for Perlin noise
    /// </summary>
    [Serializable]
    public class PerlinNoise : NoiseType<PerlinNoise>
    {
        private static PerlinNoise _sInstance;

        private static readonly NoiseTypeDescriptor _desc = new()
        {
            Name = "Perlin",
            OutputDir = MaskFilterShadersPath.Path + "/NoiseLib/",
            SourcePath = MaskFilterShadersPath.Path + "/NoiseLib/Implementation/PerlinImpl.hlsl",
            SupportedDimensions = NoiseDimensionFlags._1D | NoiseDimensionFlags._2D | NoiseDimensionFlags._3D,
            InputStructDefinition = null
        };

        public static PerlinNoise Instance
        {
            get
            {
                if (_sInstance == null)
                {
                    _sInstance = new PerlinNoise();
                }

                return _sInstance;
            }
        }

        public override NoiseTypeDescriptor GetDescription() => _desc;
    }
}
