#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEngine;
using VladislavTsurikov.ColliderSystem.Runtime;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.PrototypeGameObject;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.Utility;
using VladislavTsurikov.PhysicsSimulator.Runtime;
using VladislavTsurikov.UnityUtility.Runtime;
using Component = VladislavTsurikov.ComponentStack.Runtime.Core.Component;

namespace VladislavTsurikov.MegaWorld.Editor.PhysicsEffectsTool.PhysicsEffectsSystem
{
    public abstract class PhysicsEffect : Component
    {
        public float PositionOffsetY = 0;
        public float Size = 10.0F;

        public void DoEffect(RayHit rayHit)
        {
            var prefabList = new List<GameObject>();

            GetGameObjectFromPhysicsOverlapSphere(rayHit, prefabRoot =>
            {
                prefabList.Add(prefabRoot);

                ApplyEffectIfNecessary(prefabRoot, GetPositionOffsetY(rayHit));

                return true;
            });

            PhysicsSimulator.Runtime.PhysicsSimulator.UseAccelerationPhysics = false;
            PhysicsSimulator.Runtime.PhysicsSimulator.SetDisablePhysicsMode<GlobalTimeDisablePhysicsMode>();
            PhysicsSimulator.Runtime.PhysicsSimulator.ResetDisablePhysicsMode();
        }

        private void ApplyEffectIfNecessary(GameObject prefabRoot, Vector3 positionOffsetY)
        {
            List<PrototypeGameObject> prototypes = GetPrototypeUtility.GetPrototypes<PrototypeGameObject>(prefabRoot);

            foreach (PrototypeGameObject proto in prototypes)
            {
                if (proto != null && proto.Selected)
                {
                    var distanceFromObject = Vector3.Distance(positionOffsetY, prefabRoot.transform.position);

                    if (distanceFromObject < Size / 2)
                    {
                        SimulatedBody simulatedBody = GetSimulatedBody(prefabRoot);
                        if (simulatedBody.Rigidbody != null)
                        {
                            ApplyEffect(positionOffsetY, simulatedBody.Rigidbody);
                        }
                    }
                }
            }
        }

        private SimulatedBody GetSimulatedBody(GameObject prefabRoot)
        {
            if (SimulatedBodyStack.GetSimulatedBody(prefabRoot) == null)
            {
                var simulatedBody = new SimulatedBody(prefabRoot);
                SimulatedBodyStack.RegisterSimulatedBody(simulatedBody);

                return simulatedBody;
            }
            else
            {
                SimulatedBody simulatedBody = SimulatedBodyStack.GetSimulatedBody(prefabRoot);
                return simulatedBody;
            }
        }

        private void GetGameObjectFromPhysicsOverlapSphere(RayHit rayHit, Func<GameObject, bool> func)
        {
            Collider[] colliders = Physics.OverlapSphere(GetPositionOffsetY(rayHit), Size / 2);
            foreach (Collider hit in colliders)
            {
                GameObject prefabRoot = GameObjectUtility.GetPrefabRoot(hit.gameObject);
                if (prefabRoot == null)
                {
                    continue;
                }

                func.Invoke(prefabRoot);
            }
        }

        protected Vector3 GetPositionOffsetY(RayHit rayHit)
        {
            Vector3 positionOffsetY = rayHit.Point;
            positionOffsetY.y += PositionOffsetY;

            return positionOffsetY;
        }

        protected float GetOpacity(float force, float maxForce)
        {
            var difference = force / maxForce;
            difference = Mathf.InverseLerp(0, 0.7f, difference);

            difference = difference < 0.3 ? 0.3f : difference;

            return difference * 0.6f;
        }

        public virtual void ApplyEffect(Vector3 positionOffsetY, Rigidbody rb)
        {
        }

        public virtual void OnRepaint(RayHit rayHit)
        {
        }
    }
}
#endif
