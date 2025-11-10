using UnityEngine;

namespace VladislavTsurikov.CPUNoise.Runtime
{
    /// <summary>
    ///     Simple noise implementation by interpolating random values.
    ///     Works same as Perlin noise but uses the values instead of gradients.
    ///     Perlin noise uses gradients as it makes better noise but this still
    ///     looks good and might be a little faster.
    /// </summary>
    public class ValueNoiseCPU : NoiseCPU
    {
        public ValueNoiseCPU(int seed, float frequency, float amplitude = 1.0f)
        {
            Frequency = frequency;
            Amplitude = amplitude;
            Offset = Vector3.zero;

            Perm = new PermutationTable(1024, 255, seed);
        }

        private PermutationTable Perm { get; }

        /// <summary>
        ///     Update the seed.
        /// </summary>
        public override void UpdateSeed(int seed) => Perm.Build(seed);

        /// <summary>
        ///     Sample the noise in 1 dimension.
        /// </summary>
        public override float Sample1D(float x)
        {
            x = (x + Offset.x) * Frequency;

            var ix0 = (int)Mathf.Floor(x); // Integer part of x
            var fx0 = x - ix0; // Fractional part of x

            var s = Fade(fx0);

            float n0 = Perm[ix0];
            float n1 = Perm[ix0 + 1];

            // rescale from 0 to 255 to -1 to 1.
            var n = Lerp(s, n0, n1) * Perm.Inverse;
            n = n * 2.0f - 1.0f;

            return n * Amplitude;
        }

        /// <summary>
        ///     Sample the noise in 2 dimensions.
        /// </summary>
        public override float Sample2D(float x, float y)
        {
            x = (x + Offset.x) * Frequency;
            y = (y + Offset.y) * Frequency;

            var ix0 = (int)Mathf.Floor(x); // Integer part of x
            var iy0 = (int)Mathf.Floor(y); // Integer part of y

            var fx0 = x - ix0; // Fractional part of x
            var fy0 = y - iy0; // Fractional part of y

            var t = Fade(fy0);
            var s = Fade(fx0);

            float nx0 = Perm[ix0, iy0];
            float nx1 = Perm[ix0, iy0 + 1];

            var n0 = Lerp(t, nx0, nx1);

            nx0 = Perm[ix0 + 1, iy0];
            nx1 = Perm[ix0 + 1, iy0 + 1];

            var n1 = Lerp(t, nx0, nx1);

            // rescale from 0 to 255 to -1 to 1.
            var n = Lerp(s, n0, n1) * Perm.Inverse;
            n = n * 2.0f - 1.0f;

            return n * Amplitude;
        }

        /// <summary>
        ///     Sample the noise in 3 dimensions.
        /// </summary>
        public override float Sample3D(float x, float y, float z)
        {
            x = (x + Offset.x) * Frequency;
            y = (y + Offset.y) * Frequency;
            z = (z + Offset.z) * Frequency;

            var ix0 = (int)Mathf.Floor(x); // Integer part of x
            var iy0 = (int)Mathf.Floor(y); // Integer part of y
            var iz0 = (int)Mathf.Floor(z); // Integer part of z
            var fx0 = x - ix0; // Fractional part of x
            var fy0 = y - iy0; // Fractional part of y
            var fz0 = z - iz0; // Fractional part of z

            var r = Fade(fz0);
            var t = Fade(fy0);
            var s = Fade(fx0);

            float nxy0 = Perm[ix0, iy0, iz0];
            float nxy1 = Perm[ix0, iy0, iz0 + 1];
            var nx0 = Lerp(r, nxy0, nxy1);

            nxy0 = Perm[ix0, iy0 + 1, iz0];
            nxy1 = Perm[ix0, iy0 + 1, iz0 + 1];
            var nx1 = Lerp(r, nxy0, nxy1);

            var n0 = Lerp(t, nx0, nx1);

            nxy0 = Perm[ix0 + 1, iy0, iz0];
            nxy1 = Perm[ix0 + 1, iy0, iz0 + 1];
            nx0 = Lerp(r, nxy0, nxy1);

            nxy0 = Perm[ix0 + 1, iy0 + 1, iz0];
            nxy1 = Perm[ix0 + 1, iy0 + 1, iz0 + 1];
            nx1 = Lerp(r, nxy0, nxy1);

            var n1 = Lerp(t, nx0, nx1);

            // rescale from 0 to 255 to -1 to 1.
            var n = Lerp(s, n0, n1) * Perm.Inverse;
            n = n * 2.0f - 1.0f;

            return n * Amplitude;
        }

        private float Fade(float t) => t * t * t * (t * (t * 6.0f - 15.0f) + 10.0f);

        private float Lerp(float t, float a, float b) => a + t * (b - a);
    }
}
