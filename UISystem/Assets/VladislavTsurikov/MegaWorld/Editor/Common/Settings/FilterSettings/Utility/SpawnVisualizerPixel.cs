using UnityEngine;

namespace VladislavTsurikov.MegaWorld.Editor.Common.Settings.FilterSettings
{
    public class SpawnVisualizerPixel
    {
        public readonly float Alpha;
        public readonly float Fitness;
        public Vector3 Position;

        public SpawnVisualizerPixel(Vector3 position, float fitness, float alpha)
        {
            Position = position;
            Fitness = fitness;
            Alpha = alpha;
        }
    }
}
