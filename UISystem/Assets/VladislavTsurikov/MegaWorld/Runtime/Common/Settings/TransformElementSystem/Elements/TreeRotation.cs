using System;
using UnityEngine;
using VladislavTsurikov.ReflectionUtility;
using VladislavTsurikov.UnityUtility.Runtime;
using Random = UnityEngine.Random;

namespace VladislavTsurikov.MegaWorld.Runtime.Common.Settings.TransformElementSystem
{
    [Serializable]
    [Name("Tree Rotation")]
    public class TreeRotation : TransformComponent
    {
        public float RandomizeOrientationY = 100;
        public float RandomizeOrientationXZ = 3;
        public float SuccessRandomizeOrientationXZ = 20;

        public override void SetInstanceData(ref Instance instance, float fitness, Vector3 normal)
        {
            Vector3 randomVector = Random.insideUnitSphere * 0.5f;

            var randomSuccess = Random.Range(0.0f, 1.0f);

            if (randomSuccess < SuccessRandomizeOrientationXZ / 100)
            {
                var randomRotation = Quaternion.Euler(new Vector3(
                    RandomizeOrientationXZ * 3.6f * randomVector.x,
                    RandomizeOrientationY * 3.6f * randomVector.y,
                    RandomizeOrientationXZ * 3.6f * randomVector.z));

                instance.Rotation *= randomRotation;
            }
            else
            {
                var randomRotation = Quaternion.Euler(new Vector3(0, RandomizeOrientationY * 3.6f * randomVector.y, 0));

                instance.Rotation *= randomRotation;
            }
        }
    }
}
