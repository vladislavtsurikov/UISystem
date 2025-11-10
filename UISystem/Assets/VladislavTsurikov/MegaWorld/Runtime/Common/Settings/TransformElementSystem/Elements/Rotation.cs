using System;
using UnityEngine;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.TransformElementSystem.Attributes;
using VladislavTsurikov.ReflectionUtility;
using VladislavTsurikov.UnityUtility.Runtime;
using Random = UnityEngine.Random;

namespace VladislavTsurikov.MegaWorld.Runtime.Common.Settings.TransformElementSystem
{
    [Serializable]
    [Simple]
    [Name("Rotation")]
    public class Rotation : TransformComponent
    {
        public float RandomizeOrientationX = 5;
        public float RandomizeOrientationY = 100;
        public float RandomizeOrientationZ = 5;

        public override void SetInstanceData(ref Instance instance, float fitness, Vector3 normal)
        {
            Vector3 randomVector = Random.insideUnitSphere * 0.5f;
            var randomRotation = Quaternion.Euler(new Vector3(
                RandomizeOrientationX * 3.6f * randomVector.x,
                RandomizeOrientationY * 3.6f * randomVector.y,
                RandomizeOrientationZ * 3.6f * randomVector.z));

            instance.Rotation = randomRotation;
        }
    }
}
