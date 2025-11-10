using UnityEngine;

namespace VladislavTsurikov.MegaWorld.Runtime.Common
{
    public class PastTransform
    {
        public PastTransform(Transform transform)
        {
            Position = transform.position;
            Scale = transform.lossyScale;
            Rotation = transform.rotation;
        }

        public Vector3 Position { get; }
        public Vector3 Scale { get; }
        public Quaternion Rotation { get; }
    }
}
