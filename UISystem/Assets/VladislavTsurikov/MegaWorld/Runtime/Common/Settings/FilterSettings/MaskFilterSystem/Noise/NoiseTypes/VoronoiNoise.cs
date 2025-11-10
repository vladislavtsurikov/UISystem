using System;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.FilterSettings.MaskFilterSystem.Noise.API;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.FilterSettings.MaskFilterSystem.Shaders;

namespace VladislavTsurikov.MegaWorld.Runtime.Common.Settings.FilterSettings.MaskFilterSystem.Noise.NoiseTypes
{
    /// <summary>
    ///     A NoiseType implementation for Voronoi noise
    /// </summary>
    [Serializable]
    public class VoronoiNoise : NoiseType<VoronoiNoise>
    {
        private static VoronoiNoise _sInstance;

        private static readonly NoiseTypeDescriptor _desc = new()
        {
            Name = "Voronoi",
            OutputDir = MaskFilterShadersPath.Path + "/NoiseLib/",
            SourcePath = MaskFilterShadersPath.Path + "/NoiseLib/Implementation/VoronoiImpl.hlsl",
            SupportedDimensions = NoiseDimensionFlags._1D | NoiseDimensionFlags._2D | NoiseDimensionFlags._3D,
            InputStructDefinition = null
        };

        public static VoronoiNoise Instance
        {
            get
            {
                if (_sInstance == null)
                {
                    _sInstance = new VoronoiNoise();
                }

                return _sInstance;
            }
        }

        public override NoiseTypeDescriptor GetDescription() => _desc;
    }
}
