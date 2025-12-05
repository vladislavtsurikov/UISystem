using System;
using UnityEngine;
using VladislavTsurikov.ReflectionUtility;
using VladislavTsurikov.UnityUtility.Runtime;

namespace VladislavTsurikov.MegaWorld.Runtime.Common.Settings.TransformElementSystem
{
    [Serializable]
    [Name("Scale Clamp")]
    public class ScaleClamp : TransformComponent
    {
        public float MaxScale = 2;
        public float MinScale = 0.5f;

        public override void SetInstanceData(ref Instance instance, float fitness, Vector3 normal)
        {
            if (instance.Scale.x > MaxScale)
            {
                instance.Scale.x = MaxScale;
            }
            else if (instance.Scale.x < MinScale)
            {
                instance.Scale.x = MinScale;
            }

            if (instance.Scale.y > MaxScale)
            {
                instance.Scale.y = MaxScale;
            }
            else if (instance.Scale.y < MinScale)
            {
                instance.Scale.y = MinScale;
            }

            if (instance.Scale.z > MaxScale)
            {
                instance.Scale.z = MaxScale;
            }
            else if (instance.Scale.z < MinScale)
            {
                instance.Scale.z = MinScale;
            }
        }
    }
}
