#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.ColliderSystem.Runtime;
using VladislavTsurikov.ReflectionUtility;

namespace VladislavTsurikov.MegaWorld.Editor.PhysicsEffectsTool.PhysicsEffectsSystem
{
    [Name("Black Hole")]
    public class BlackHole : PhysicsEffect
    {
        public float Force = 50F;
        public float MaxForce => 500;

        public override void ApplyEffect(Vector3 positionOffsetY, Rigidbody rb) =>
            rb.AddForce(Force * (positionOffsetY - rb.transform.position) /
                        Vector3.Distance(positionOffsetY, rb.transform.position));

        public override void OnRepaint(RayHit rayHit)
        {
            Vector3 positionUp = GetPositionOffsetY(rayHit);

            Handles.color = new Vector4(0, 0, 0, GetOpacity(Force, MaxForce));

            Handles.SphereHandleCap(0, positionUp, Quaternion.identity, Size, EventType.Repaint);
            Handles.color = Color.black;
            Handles.DrawDottedLine(rayHit.Point, positionUp, 2f);
            Handles.SphereHandleCap(0, positionUp, Quaternion.identity, 0.3f, EventType.Repaint);
        }
    }
}
#endif
