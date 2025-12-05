using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace VladislavTsurikov.MegaWorld.Runtime.Common.Settings.FilterSettings.MaskFilterSystem.Noise.API
{
    /// <summary>
    ///     A utility class for rendering noise defined by NoiseSettings into various Texture types.
    /// </summary>
    public static class NoiseUtils
    {
        // private static readonly int kAllSlices = -1;

        /// <summary>
        ///     Number of passes that the builtin noise blit shader contains. The passes include
        ///     one for blitting 2D noise and one for blitting 3D noise.
        /// </summary>
        public static readonly int KNumBlitPasses = 2;

        /*=========================================================================

            Material Getter Functions

        =========================================================================*/

        private static Material _sDefaultPreviewMaterial;

        static NoiseUtils()
        {
        }

        private static bool supportsCopyTexture3D => (SystemInfo.copyTextureSupport & CopyTextureSupport.Copy3D) != 0;

        private static bool supportsCopyTextureRTToTexture =>
            (SystemInfo.copyTextureSupport & CopyTextureSupport.RTToTexture) != 0;

        private static Material GetDefaultPreviewMaterial()
        {
            if (_sDefaultPreviewMaterial == null)
            {
                _sDefaultPreviewMaterial = new Material(Shader.Find("Hidden/TerrainTools/Noise/Preview"));
            }

            return _sDefaultPreviewMaterial;
        }

        /// <summary>
        ///     Returns a Material reference to the default blit material for the given Type of FractalType.
        /// </summary>
        /// <remarks> Usage is: Material mat = GetDefaultBlitMaterial( typeof(FbmFractalType) ); </remarks>
        /// <returns>
        ///     A reference to the default blit Material for the specified Type of FractalType </return>
        ///     <param name="fractalType"> The Type for a given FractalType </param>
        public static Material GetDefaultBlitMaterial(Type fractalType) =>
            NoiseLib.GetGeneratedMaterial(typeof(NoiseBlitShaderGenerator), fractalType);

        /// <summary>
        ///     Returns a Material reference to the default blit material for the given NoiseSettings object.
        /// </summary>
        /// <remarks>
        ///     Internally, this uses noise.domainSettings.fractalTypeName to get it's FractalType
        /// </remarks>
        /// <returns> A reference to the default blit Material for the specified NoiseSettings instance </returns>
        public static Material GetDefaultBlitMaterial(NoiseSettings noise)
        {
            if (noise.DomainSettings == null)
            {
                Debug.Log("null");
            }

            IFractalType fractal = NoiseLib.GetFractalTypeInstance(noise.DomainSettings.FractalTypeName);

            if (fractal == null)
            {
                return null;
            }

            return GetDefaultBlitMaterial(fractal.GetType());
        }

        /*=========================================================================

            Blit Noise using Preview shader

        =========================================================================*/

        /// <summary>
        ///     Blits the source RenderTexture into the destination RenderTexture using the default Preview Blit Material.
        ///     This is the blit Material that is used when rendering the NoiseSettings Preview.
        /// </summary>
        /// <param name="source"> The source RenderTexture used in the Blit operation </param>
        /// <param name="destination">
        ///     The destination RenderTexture in the Blit operation
        ///     This will have the noise preview shader logic applied to it.
        /// </param>
        public static void BlitPreview2D(RenderTexture source, RenderTexture destination) =>
            BlitPreview2D(source, destination, GetDefaultPreviewMaterial());

        /// <summary>
        ///     Blits the source RenderTexture into the destination RenderTexture using the specified Material.
        /// </summary>
        /// <param name="source"> The source RenderTexture used in the Blit operation </param>
        /// <param name="destination">
        ///     The destination RenderTexture in the Blit operation
        ///     This will have the noise preview shader logic applied to it.
        /// </param>
        /// <param name="mat"> The material to be used in the blit operation </param>
        public static void BlitPreview2D(RenderTexture src, RenderTexture dest, Material mat)
        {
            if (mat == null)
            {
                Debug.LogError("NoiseUtils::BlitPreview2D: Provided preview material is NULL");

                Graphics.Blit(src, dest);

                return;
            }

            Graphics.Blit(src, dest, mat, 0);
        }

        /*=========================================================================

            Blit
                - Blit raw noise data into texture

        =========================================================================*/

        /// <summary>
        ///     Blits 2D noise defined by the given NoiseSettings instance into the destination RenderTexture.
        /// </summary>
        /// <param name="noise"> An instance of NoiseSettings defining the type of noise to render </param>
        /// <param name="dest"> The destination RenderTexture that the noise will be rendered into. </param>
        public static void Blit2D(NoiseSettings noise, RenderTexture dest)
        {
            Material mat = GetDefaultBlitMaterial(noise);

            if (mat == null)
            {
                return;
            }

            Blit2D(noise, dest, mat);
        }

        /// <summary>
        ///     Blits 2D noise defined by the given NoiseSettings instance into the destination RenderTexture
        ///     using the provided Material.
        /// </summary>
        /// <param name="noise"> An instance of NoiseSettings defining the type of noise to render </param>
        /// <param name="dest"> The destination RenderTexture that the noise will be rendered into. </param>
        /// <param name="mat"> The Material to be used for rendering the noise </param>
        public static void Blit2D(NoiseSettings noise, RenderTexture dest, Material mat)
        {
            var pass = NoiseLib.GetNoiseIndex(noise.DomainSettings.NoiseTypeName);

            INTERNAL_Blit2D(noise, dest, mat, pass * KNumBlitPasses + 0);
        }

        private static void INTERNAL_Blit2D(NoiseSettings noise, RenderTexture dest, Material mat, int pass)
        {
            noise.SetupMaterial(mat);

            var tempRT = RenderTexture.GetTemporary(dest.descriptor);
            RenderTexture prev = RenderTexture.active;
            RenderTexture.active = tempRT;

            Graphics.Blit(tempRT, mat, pass);

            RenderTexture.active = dest;

            // if(noise.filterSettings.filterStack != null)
            // {
            //     noise.filterSettings.filterStack.Eval(tempRT, dest);
            // }
            // else
            {
                Graphics.Blit(tempRT, dest);
            }

            RenderTexture.active = prev;

            RenderTexture.ReleaseTemporary(tempRT);
        }
    }
}
