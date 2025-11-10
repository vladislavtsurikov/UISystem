using System;
using UnityEngine;

namespace VladislavTsurikov.CPUNoise.Runtime
{
    public class WorleyNoiseCPU : NoiseCPU
    {
        private const float K = 1.0f / 7.0f;

        private const float Ko = 3.0f / 7.0f;

        private static readonly float[] offsetF = { -0.5f, 0.5f, 1.5f };

        public WorleyNoiseCPU(int seed, float frequency, float jitter, float amplitude = 1.0f)
        {
            Frequency = frequency;
            Amplitude = amplitude;
            Offset = Vector3.zero;
            Jitter = jitter;
            Distance = VoronoiDistance.Euclidian;
            Combination = VoronoiCombination.D1D0;

            Perm = new PermutationTable(1024, 255, seed);
        }

        public float Jitter { get; set; }

        public VoronoiDistance Distance { get; set; }

        public VoronoiCombination Combination { get; set; }

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

            var pi0 = (int)Mathf.Floor(x);
            var pf0 = Frac(x);

            var pX = new Vector3 { [0] = Perm[pi0 - 1], [1] = Perm[pi0], [2] = Perm[pi0 + 1] };

            var f0 = float.PositiveInfinity;
            var f1 = float.PositiveInfinity;
            var f2 = float.PositiveInfinity;

            var px = Perm[(int)pX[0]];
            var py = Perm[(int)pX[1]];
            var pz = Perm[(int)pX[2]];

            var oxx = Frac(px * K) - Ko;
            var oxy = Frac(py * K) - Ko;
            var oxz = Frac(pz * K) - Ko;

            var d0 = Distance1(pf0, offsetF[0] + Jitter * oxx);
            var d1 = Distance1(pf0, offsetF[1] + Jitter * oxy);
            var d2 = Distance1(pf0, offsetF[2] + Jitter * oxz);

            if (d0 < f0)
            {
                f2 = f1;
                f1 = f0;
                f0 = d0;
            }
            else if (d0 < f1)
            {
                f2 = f1;
                f1 = d0;
            }
            else if (d0 < f2)
            {
                f2 = d0;
            }

            if (d1 < f0)
            {
                f2 = f1;
                f1 = f0;
                f0 = d1;
            }
            else if (d1 < f1)
            {
                f2 = f1;
                f1 = d1;
            }
            else if (d1 < f2)
            {
                f2 = d1;
            }

            if (d2 < f0)
            {
                f2 = f1;
                f1 = f0;
                f0 = d2;
            }
            else if (d2 < f1)
            {
                f2 = f1;
                f1 = d2;
            }
            else if (d2 < f2)
            {
                f2 = d2;
            }

            return Combine(f0, f1, f2) * Amplitude;
        }

        /// <summary>
        ///     Sample the noise in 2 dimensions.
        /// </summary>
        public override float Sample2D(float x, float y)
        {
            x = (x + Offset.x) * Frequency;
            y = (y + Offset.y) * Frequency;

            var pi0 = (int)Mathf.Floor(x);
            var pi1 = (int)Mathf.Floor(y);

            var pf0 = Frac(x);
            var pf1 = Frac(y);

            var pX = new Vector3 { [0] = Perm[pi0 - 1], [1] = Perm[pi0], [2] = Perm[pi0 + 1] };

            float d0, d1, d2;
            var f0 = float.PositiveInfinity;
            var f1 = float.PositiveInfinity;
            var f2 = float.PositiveInfinity;

            int px, py, pz;
            float oxx, oxy, oxz;
            float oyx, oyy, oyz;

            for (var i = 0; i < 3; i++)
            {
                px = Perm[(int)pX[i], pi1 - 1];
                py = Perm[(int)pX[i], pi1];
                pz = Perm[(int)pX[i], pi1 + 1];

                oxx = Frac(px * K) - Ko;
                oxy = Frac(py * K) - Ko;
                oxz = Frac(pz * K) - Ko;

                oyx = Mod(Mathf.Floor(px * K), 7.0f) * K - Ko;
                oyy = Mod(Mathf.Floor(py * K), 7.0f) * K - Ko;
                oyz = Mod(Mathf.Floor(pz * K), 7.0f) * K - Ko;

                d0 = Distance2(pf0, pf1, offsetF[i] + Jitter * oxx, -0.5f + Jitter * oyx);
                d1 = Distance2(pf0, pf1, offsetF[i] + Jitter * oxy, 0.5f + Jitter * oyy);
                d2 = Distance2(pf0, pf1, offsetF[i] + Jitter * oxz, 1.5f + Jitter * oyz);

                if (d0 < f0)
                {
                    f2 = f1;
                    f1 = f0;
                    f0 = d0;
                }
                else if (d0 < f1)
                {
                    f2 = f1;
                    f1 = d0;
                }
                else if (d0 < f2)
                {
                    f2 = d0;
                }

                if (d1 < f0)
                {
                    f2 = f1;
                    f1 = f0;
                    f0 = d1;
                }
                else if (d1 < f1)
                {
                    f2 = f1;
                    f1 = d1;
                }
                else if (d1 < f2)
                {
                    f2 = d1;
                }

                if (d2 < f0)
                {
                    f2 = f1;
                    f1 = f0;
                    f0 = d2;
                }
                else if (d2 < f1)
                {
                    f2 = f1;
                    f1 = d2;
                }
                else if (d2 < f2)
                {
                    f2 = d2;
                }
            }

            return Combine(f0, f1, f2) * Amplitude;
        }

        /// <summary>
        ///     Sample the noise in 3 dimensions.
        /// </summary>
        public override float Sample3D(float x, float y, float z)
        {
            x = (x + Offset.x) * Frequency;
            y = (y + Offset.y) * Frequency;
            z = (z + Offset.z) * Frequency;

            var pi0 = (int)Mathf.Floor(x);
            var pi1 = (int)Mathf.Floor(y);
            var pi2 = (int)Mathf.Floor(z);

            var pf0 = Frac(x);
            var pf1 = Frac(y);
            var pf2 = Frac(z);

            var pX = new Vector3 { [0] = Perm[pi0 - 1], [1] = Perm[pi0], [2] = Perm[pi0 + 1] };

            var pY = new Vector3 { [0] = Perm[pi1 - 1], [1] = Perm[pi1], [2] = Perm[pi1 + 1] };

            float d0, d1, d2;
            var f0 = 1e6f;
            var f1 = 1e6f;
            var f2 = 1e6f;

            int px, py, pz;
            float oxx, oxy, oxz;
            float oyx, oyy, oyz;
            float ozx, ozy, ozz;

            for (var i = 0; i < 3; i++)
            for (var j = 0; j < 3; j++)
            {
                px = Perm[(int)pX[i], (int)pY[j], pi2 - 1];
                py = Perm[(int)pX[i], (int)pY[j], pi2];
                pz = Perm[(int)pX[i], (int)pY[j], pi2 + 1];

                oxx = Frac(px * K) - Ko;
                oxy = Frac(py * K) - Ko;
                oxz = Frac(pz * K) - Ko;

                oyx = Mod(Mathf.Floor(px * K), 7.0f) * K - Ko;
                oyy = Mod(Mathf.Floor(py * K), 7.0f) * K - Ko;
                oyz = Mod(Mathf.Floor(pz * K), 7.0f) * K - Ko;

                px = Perm[px];
                py = Perm[py];
                pz = Perm[pz];

                ozx = Frac(px * K) - Ko;
                ozy = Frac(py * K) - Ko;
                ozz = Frac(pz * K) - Ko;

                d0 = Distance3(pf0, pf1, pf2, offsetF[i] + Jitter * oxx, offsetF[j] + Jitter * oyx,
                    -0.5f + Jitter * ozx);
                d1 = Distance3(pf0, pf1, pf2, offsetF[i] + Jitter * oxy, offsetF[j] + Jitter * oyy,
                    0.5f + Jitter * ozy);
                d2 = Distance3(pf0, pf1, pf2, offsetF[i] + Jitter * oxz, offsetF[j] + Jitter * oyz,
                    1.5f + Jitter * ozz);

                if (d0 < f0)
                {
                    f2 = f1;
                    f1 = f0;
                    f0 = d0;
                }
                else if (d0 < f1)
                {
                    f2 = f1;
                    f1 = d0;
                }
                else if (d0 < f2)
                {
                    f2 = d0;
                }

                if (d1 < f0)
                {
                    f2 = f1;
                    f1 = f0;
                    f0 = d1;
                }
                else if (d1 < f1)
                {
                    f2 = f1;
                    f1 = d1;
                }
                else if (d1 < f2)
                {
                    f2 = d1;
                }

                if (d2 < f0)
                {
                    f2 = f1;
                    f1 = f0;
                    f0 = d2;
                }
                else if (d2 < f1)
                {
                    f2 = f1;
                    f1 = d2;
                }
                else if (d2 < f2)
                {
                    f2 = d2;
                }
            }

            return Combine(f0, f1, f2) * Amplitude;
        }

        private float Mod(float x, float y) => x - y * Mathf.Floor(x / y);

        private float Frac(float v) => v - Mathf.Floor(v);

        private float Distance1(float p1X, float p2X)
        {
            switch (Distance)
            {
                case VoronoiDistance.Euclidian:
                    return (p1X - p2X) * (p1X - p2X);

                case VoronoiDistance.Manhattan:
                    return Math.Abs(p1X - p2X);

                case VoronoiDistance.Chebyshev:
                    return Math.Abs(p1X - p2X);
            }

            return 0;
        }

        private float Distance2(float p1X, float p1Y, float p2X, float p2Y)
        {
            switch (Distance)
            {
                case VoronoiDistance.Euclidian:
                    return (p1X - p2X) * (p1X - p2X) + (p1Y - p2Y) * (p1Y - p2Y);

                case VoronoiDistance.Manhattan:
                    return Math.Abs(p1X - p2X) + Math.Abs(p1Y - p2Y);

                case VoronoiDistance.Chebyshev:
                    return Math.Max(Math.Abs(p1X - p2X), Math.Abs(p1Y - p2Y));
            }

            return 0;
        }

        private float Distance3(float p1X, float p1Y, float p1Z, float p2X, float p2Y, float p2Z)
        {
            switch (Distance)
            {
                case VoronoiDistance.Euclidian:
                    return (p1X - p2X) * (p1X - p2X) + (p1Y - p2Y) * (p1Y - p2Y) + (p1Z - p2Z) * (p1Z - p2Z);

                case VoronoiDistance.Manhattan:
                    return Math.Abs(p1X - p2X) + Math.Abs(p1Y - p2Y) + Math.Abs(p1Z - p2Z);

                case VoronoiDistance.Chebyshev:
                    return Math.Max(Math.Max(Math.Abs(p1X - p2X), Math.Abs(p1Y - p2Y)), Math.Abs(p1Z - p2Z));
            }

            return 0;
        }

        private float Combine(float f0, float f1, float f2)
        {
            switch (Combination)
            {
                case VoronoiCombination.D0:
                    return f0;

                case VoronoiCombination.D1D0:
                    return f1 - f0;

                case VoronoiCombination.D2D0:
                    return f2 - f0;
            }

            return 0;
        }
    }
}
