using System;
using UnityEngine;

namespace VladislavTsurikov.CPUNoise.Runtime
{
    public enum VoronoiDistance
    {
        Euclidian,
        Manhattan,
        Chebyshev
    }

    public enum VoronoiCombination
    {
        D0,
        D1D0,
        D2D0
    }

    public class VoronoiNoiseCPU : NoiseCPU
    {
        public VoronoiNoiseCPU(int seed, float frequency, float amplitude = 1.0f)
        {
            Frequency = frequency;
            Amplitude = amplitude;
            Offset = Vector3.zero;

            Distance = VoronoiDistance.Euclidian;
            Combination = VoronoiCombination.D1D0;

            Perm = new PermutationTable(1024, int.MaxValue, seed);
        }

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
            //The 0.75 is to make the scale simliar to the other noise algorithms
            x = (x + Offset.x) * Frequency * 0.75f;

            int lastRandom, numberFeaturePoints;
            float randomDiffX;
            float featurePointX;
            int cubeX;

            var distanceArray = new Vector3(float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity);

            //1. Determine which cube the evaluation point is in
            var evalCubeX = (int)Mathf.Floor(x);

            for (var i = -1; i < 2; ++i)
            {
                cubeX = evalCubeX + i;

                //2. Generate a reproducible random number generator for the cube
                lastRandom = Perm[cubeX];

                //3. Determine how many feature points are in the cube
                numberFeaturePoints = ProbLookup(lastRandom * Perm.Inverse);

                //4. Randomly place the feature points in the cube
                for (var l = 0; l < numberFeaturePoints; ++l)
                {
                    lastRandom = Perm[lastRandom];
                    randomDiffX = lastRandom * Perm.Inverse;

                    lastRandom = Perm[lastRandom];

                    featurePointX = randomDiffX + cubeX;

                    //5. Find the feature point closest to the evaluation point. 
                    //This is done by inserting the distances to the feature points into a sorted list
                    distanceArray = Insert(distanceArray, Distance1(x, featurePointX));
                }

                //6. Check the neighboring cubes to ensure their are no closer evaluation points.
                // This is done by repeating steps 1 through 5 above for each neighboring cube
            }

            return Combine(distanceArray) * Amplitude;
        }

        /// <summary>
        ///     Sample the noise in 2 dimensions.
        /// </summary>
        public override float Sample2D(float x, float y)
        {
            //The 0.75 is to make the scale simliar to the other noise algorithms
            x = (x + Offset.x) * Frequency * 0.75f;
            y = (y + Offset.y) * Frequency * 0.75f;

            int lastRandom, numberFeaturePoints;
            float randomDiffX, randomDiffY;
            float featurePointX, featurePointY;
            int cubeX, cubeY;

            var distanceArray = new Vector3(float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity);

            //1. Determine which cube the evaluation point is in
            var evalCubeX = (int)Mathf.Floor(x);
            var evalCubeY = (int)Mathf.Floor(y);

            for (var i = -1; i < 2; ++i)
            for (var j = -1; j < 2; ++j)
            {
                cubeX = evalCubeX + i;
                cubeY = evalCubeY + j;

                //2. Generate a reproducible random number generator for the cube
                lastRandom = Perm[cubeX, cubeY];

                //3. Determine how many feature points are in the cube
                numberFeaturePoints = ProbLookup(lastRandom * Perm.Inverse);

                //4. Randomly place the feature points in the cube
                for (var l = 0; l < numberFeaturePoints; ++l)
                {
                    lastRandom = Perm[lastRandom];
                    randomDiffX = lastRandom * Perm.Inverse;

                    lastRandom = Perm[lastRandom];
                    randomDiffY = lastRandom * Perm.Inverse;

                    featurePointX = randomDiffX + cubeX;
                    featurePointY = randomDiffY + cubeY;

                    //5. Find the feature point closest to the evaluation point. 
                    //This is done by inserting the distances to the feature points into a sorted list
                    distanceArray = Insert(distanceArray, Distance2(x, y, featurePointX, featurePointY));
                }
                //6. Check the neighboring cubes to ensure their are no closer evaluation points.
                // This is done by repeating steps 1 through 5 above for each neighboring cube
            }

            return Combine(distanceArray) * Amplitude;
        }

        /// <summary>
        ///     Sample the noise in 3 dimensions.
        /// </summary>
        public override float Sample3D(float x, float y, float z)
        {
            //The 0.75 is to make the scale simliar to the other noise algorithms
            x = (x + Offset.x) * Frequency * 0.75f;
            y = (y + Offset.y) * Frequency * 0.75f;
            z = (z + Offset.z) * Frequency * 0.75f;

            int lastRandom, numberFeaturePoints;
            float randomDiffX, randomDiffY, randomDiffZ;
            float featurePointX, featurePointY, featurePointZ;
            int cubeX, cubeY, cubeZ;

            var distanceArray = new Vector3(float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity);

            //1. Determine which cube the evaluation point is in
            var evalCubeX = (int)Mathf.Floor(x);
            var evalCubeY = (int)Mathf.Floor(y);
            var evalCubeZ = (int)Mathf.Floor(z);

            for (var i = -1; i < 2; ++i)
            for (var j = -1; j < 2; ++j)
            for (var k = -1; k < 2; ++k)
            {
                cubeX = evalCubeX + i;
                cubeY = evalCubeY + j;
                cubeZ = evalCubeZ + k;

                //2. Generate a reproducible random number generator for the cube
                lastRandom = Perm[cubeX, cubeY, cubeZ];

                //3. Determine how many feature points are in the cube
                numberFeaturePoints = ProbLookup(lastRandom * Perm.Inverse);

                //4. Randomly place the feature points in the cube
                for (var l = 0; l < numberFeaturePoints; ++l)
                {
                    lastRandom = Perm[lastRandom];
                    randomDiffX = lastRandom * Perm.Inverse;

                    lastRandom = Perm[lastRandom];
                    randomDiffY = lastRandom * Perm.Inverse;

                    lastRandom = Perm[lastRandom];
                    randomDiffZ = lastRandom * Perm.Inverse;

                    featurePointX = randomDiffX + cubeX;
                    featurePointY = randomDiffY + cubeY;
                    featurePointZ = randomDiffZ + cubeZ;

                    //5. Find the feature point closest to the evaluation point. 
                    //This is done by inserting the distances to the feature points into a sorted list
                    distanceArray = Insert(distanceArray,
                        Distance3(x, y, z, featurePointX, featurePointY, featurePointZ));
                }
                //6. Check the neighboring cubes to ensure their are no closer evaluation points.
                // This is done by repeating steps 1 through 5 above for each neighboring cube
            }

            return Combine(distanceArray) * Amplitude;
        }

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

        private float Combine(Vector3 arr)
        {
            switch (Combination)
            {
                case VoronoiCombination.D0:
                    return arr[0];

                case VoronoiCombination.D1D0:
                    return arr[1] - arr[0];

                case VoronoiCombination.D2D0:
                    return arr[2] - arr[0];
            }

            return 0;
        }

        /// <summary>
        ///     Given a uniformly distributed random number this function returns the number of feature points in a given cube.
        /// </summary>
        /// <param name="value">a uniformly distributed random number</param>
        /// <returns>The number of feature points in a cube.</returns>
        private int ProbLookup(float value)
        {
            //Poisson Distribution
            if (value < 0.0915781944272058)
            {
                return 1;
            }

            if (value < 0.238103305510735)
            {
                return 2;
            }

            if (value < 0.433470120288774)
            {
                return 3;
            }

            if (value < 0.628836935299644)
            {
                return 4;
            }

            if (value < 0.785130387122075)
            {
                return 5;
            }

            if (value < 0.889326021747972)
            {
                return 6;
            }

            if (value < 0.948866384324819)
            {
                return 7;
            }

            if (value < 0.978636565613243)
            {
                return 8;
            }

            return 9;
        }

        /// <summary>
        ///     Inserts value into array using insertion sort. If the value is greater than the largest value in the array
        ///     it will not be added to the array.
        /// </summary>
        /// <param name="arr">The array to insert the value into.</param>
        /// <param name="value">The value to insert into the array.</param>
        private Vector3 Insert(Vector3 arr, float value)
        {
            float temp;
            for (var i = 3 - 1; i >= 0; i--)
            {
                if (value > arr[i])
                {
                    break;
                }

                temp = arr[i];
                arr[i] = value;
                if (i + 1 < 3)
                {
                    arr[i + 1] = temp;
                }
            }

            return arr;
        }
    }
}
