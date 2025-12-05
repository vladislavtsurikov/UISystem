using System;
using UnityEngine;
using VladislavTsurikov.ReflectionUtility;
using VladislavTsurikov.UnityUtility.Runtime;

namespace VladislavTsurikov.MegaWorld.Runtime.Common.Settings.TransformElementSystem
{
    [Serializable]
    [Name("Additional Rotation")]
    public class AdditionalRotation : TransformComponent
    {
        public Vector3 Rotation;

        public override void SetInstanceData(ref Instance instance, float fitness, Vector3 normal) =>
            instance.Rotation *= Quaternion.Euler(Rotation);
    }
}
