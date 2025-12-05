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
    [Name("Scale")]
    public class Scale : TransformComponent
    {
        public bool UniformScale = true;
        public Vector3 MinScale = new(0.8f, 0.8f, 0.8f);
        public Vector3 MaxScale = new(1.2f, 1.2f, 1.2f);

        public override void SetInstanceData(ref Instance instance, float fitness, Vector3 normal)
        {
            if (UniformScale)
            {
                var resScale = Random.Range(MinScale.x, MaxScale.x);
                instance.Scale = new Vector3(resScale, resScale, resScale);
            }
            else
            {
                instance.Scale = new Vector3(
                    Random.Range(MinScale.x, MaxScale.x),
                    Random.Range(MinScale.y, MaxScale.y),
                    Random.Range(MinScale.z, MaxScale.z));
            }
        }
    }
}
