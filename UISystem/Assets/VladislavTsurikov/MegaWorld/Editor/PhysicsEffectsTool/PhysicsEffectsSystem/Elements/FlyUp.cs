#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.ColliderSystem.Runtime;
using VladislavTsurikov.ColorUtility.Runtime;
using VladislavTsurikov.ReflectionUtility;

namespace VladislavTsurikov.MegaWorld.Editor.PhysicsEffectsTool.PhysicsEffectsSystem
{
    [Name("Fly Up")]
    public class FlyUp : PhysicsEffect
    {
        public float Force = 30f;
        public float MaxForce => 200;

        public override void ApplyEffect(Vector3 positionOffsetY, Rigidbody rb)
        {
            Vector3 forceDirection = Vector3.up;

            forceDirection *= Force;

            rb.AddForce(forceDirection);
        }

        public override void OnRepaint(RayHit rayHit)
        {
            Vector3 positionUp = GetPositionOffsetY(rayHit);

            var colorBlue = new Color(0.2f, 0.5f, 0.7f);

            Handles.color = colorBlue.WithAlpha(GetOpacity(Force, MaxForce));
            Handles.SphereHandleCap(0, positionUp, Quaternion.identity, Size, EventType.Repaint);
            Handles.color = colorBlue;
            Handles.DrawDottedLine(rayHit.Point, positionUp, 2f);

            Handles.SphereHandleCap(0, positionUp, Quaternion.identity, 0.3f, EventType.Repaint);

            Vector3 direction = Vector3.up;
            Handles.color = colorBlue.WithAlpha(0.5f);
            Handles.ArrowHandleCap(0, positionUp, Quaternion.FromToRotation(Vector3.forward, direction), Size / 2,
                EventType.Repaint);
        }
    }
}
#endif
