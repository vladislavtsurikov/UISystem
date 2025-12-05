using System;
using UnityEngine;
using VladislavTsurikov.ReflectionUtility;
using VladislavTsurikov.UnityUtility.Runtime;

namespace VladislavTsurikov.MegaWorld.Runtime.Common.Settings.TransformElementSystem
{
    [Serializable]
    [Name("Slope Scale")]
    public class SlopeScale : TransformComponent
    {
        public bool UniformScaleOffset = true;

        [Range(0.1f, 90f)]
        public float MaxSlope = 30;

        [Min(0.1f)]
        public float MaxUniformScaleOffset = 2;

        public Vector3 MaxScaleOffset = new(2, 2, 0.5f);

        public override void SetInstanceData(ref Instance instance, float fitness, Vector3 normal)
        {
            var normalAngle = Vector3.Angle(normal, Vector3.up);
            var difference = normalAngle / MaxSlope;

            if (UniformScaleOffset)
            {
                var value = Mathf.Lerp(0, MaxUniformScaleOffset, difference);

                instance.Scale += new Vector3(value, value, value);
            }
            else
            {
                var valueX = Mathf.Lerp(0, MaxScaleOffset.x, difference);
                var valueY = Mathf.Lerp(0, MaxScaleOffset.y, difference);
                var valueZ = Mathf.Lerp(0, MaxScaleOffset.z, difference);

                instance.Scale += new Vector3(valueX, valueY, valueZ);
            }
        }
    }
}
