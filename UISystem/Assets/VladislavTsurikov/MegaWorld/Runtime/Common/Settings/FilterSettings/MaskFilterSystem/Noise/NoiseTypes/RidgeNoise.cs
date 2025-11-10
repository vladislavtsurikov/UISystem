using System;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.FilterSettings.MaskFilterSystem.Noise.API;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.FilterSettings.MaskFilterSystem.Shaders;

namespace VladislavTsurikov.MegaWorld.Runtime.Common.Settings.FilterSettings.MaskFilterSystem.Noise.NoiseTypes
{
    /// <summary>
    ///     A NoiseType implementation for Ridge noise
    /// </summary>
    [Serializable]
    public class RidgeNoise : NoiseType<RidgeNoise>
    {
        private static RidgeNoise _sInstance;

        private static readonly NoiseTypeDescriptor _desc = new()
        {
            Name = "Ridge",
            OutputDir = MaskFilterShadersPath.Path + "/NoiseLib/",
            SourcePath = MaskFilterShadersPath.Path + "/NoiseLib/Implementation/RidgeImpl.hlsl",
            SupportedDimensions = NoiseDimensionFlags._1D | NoiseDimensionFlags._2D | NoiseDimensionFlags._3D,
            InputStructDefinition = null
        };

        public static RidgeNoise Instance
        {
            get
            {
                if (_sInstance == null)
                {
                    _sInstance = new RidgeNoise();
                }

                return _sInstance;
            }
        }

        public override NoiseTypeDescriptor GetDescription() => _desc;
    }
}
