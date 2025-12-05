using UnityEngine;
using VladislavTsurikov.PhysicsSimulator.Runtime;
using VladislavTsurikov.ReflectionUtility;
using Component = VladislavTsurikov.ComponentStack.Runtime.Core.Component;

namespace VladislavTsurikov.MegaWorld.Runtime.Common.PhysXPainter.Settings
{
    [Name("Physics Effects")]
    public class PhysicsEffects : Component
    {
        #region Direction

        public float RandomStrength = 50f;

        #endregion

        public void ApplyForce(Rigidbody rigidbody)
        {
            if (rigidbody == null)
            {
                return;
            }

            var radians = Random.Range(0, 360) * Mathf.Deg2Rad;

            var forceDirection = new Vector3(Mathf.Sin(radians), 0, Mathf.Cos(radians));

            var force = Vector3.Lerp(new Vector3(0, -1, 0), forceDirection, RandomStrength / 100);

            var magnitude = ForceRange ? Random.Range(MinForce, MaxForce) : MinForce;

            force *= magnitude;

            PhysicsUtility.ApplyForce(rigidbody, force);
        }

        #region Force

        public bool ForceRange = true;
        public float MinForce = 10f;
        public float MaxForce = 40f;

        #endregion
    }
}
