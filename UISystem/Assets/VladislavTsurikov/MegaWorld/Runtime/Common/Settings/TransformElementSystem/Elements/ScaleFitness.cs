using System;
using UnityEngine;
using VladislavTsurikov.ReflectionUtility;
using VladislavTsurikov.UnityUtility.Runtime;

namespace VladislavTsurikov.MegaWorld.Runtime.Common.Settings.TransformElementSystem
{
    [Serializable]
    [Name("Scale Fitness")]
    public class ScaleFitness : TransformComponent
    {
        public float OffsetScale = -0.7f;

        public override void SetInstanceData(ref Instance instance, float fitness, Vector3 normal)
        {
            var value = Mathf.Lerp(OffsetScale, 0, fitness);

            instance.Scale += new Vector3(value, value, value);
        }
    }
}
