using UnityEngine;

namespace VladislavTsurikov.UnityUtility.Runtime
{
    public class Instance
    {
        public Vector3 Position;
        public Quaternion Rotation;
        public Vector3 Scale;

        public Instance(GameObject gameObject)
        {
            Position = gameObject.transform.position;
            Scale = gameObject.transform.localScale;
            Rotation = gameObject.transform.rotation;
        }

        public Instance(Vector3 position, Vector3 scale, Quaternion rotation)
        {
            Position = position;
            Scale = scale;
            Rotation = rotation;
        }
    }
}
