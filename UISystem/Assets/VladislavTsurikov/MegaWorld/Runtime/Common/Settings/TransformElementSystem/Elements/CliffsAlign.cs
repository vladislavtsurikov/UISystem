using System;
using UnityEngine;
using VladislavTsurikov.ReflectionUtility;
using VladislavTsurikov.UnityUtility.Runtime;

namespace VladislavTsurikov.MegaWorld.Runtime.Common.Settings.TransformElementSystem
{
    [Serializable]
    [Name("Cliffs Align")]
    public class CliffsAlign : TransformComponent
    {
        public override void SetInstanceData(ref Instance instance, float fitness, Vector3 normal)
        {
            var direction = new Vector3(normal.x, 0, normal.z);

            var distancePositive = Vector3.Distance(Vector3.right, direction);
            var distanceNegative = Vector3.Distance(-Vector3.right, direction);

            if (distancePositive < distanceNegative)
            {
                var angle = Vector3.Angle(Vector3.forward, direction);
                instance.Rotation = Quaternion.AngleAxis(angle, Vector3.up) * instance.Rotation;
            }
            else
            {
                var angle = -Vector3.Angle(Vector3.forward, direction);
                instance.Rotation = Quaternion.AngleAxis(angle, Vector3.up) * instance.Rotation;
            }
        }
    }
}
