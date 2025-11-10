using System;
using UnityEngine;
using VladislavTsurikov.UnityUtility.Runtime;

namespace VladislavTsurikov.PhysicsSimulator.Runtime
{
    [Serializable]
    public class AutoPositionDownSettings
    {
        public bool EnableAutoPositionDown = true;
        public float PositionDown = 50;

        public void ApplyOffset(GameObject go)
        {
            if (!EnableAutoPositionDown)
            {
                return;
            }

            Bounds bounds = go.GetInstantiatedBounds();

            Vector3 position = go.transform.position;
            position = new Vector3(position.x, position.y - Mathf.Lerp(0, bounds.extents.y, PositionDown / 100),
                position.z);
            go.transform.position = position;
        }
    }

    public class ApplyPositionDown : OnDisableSimulatedBodyEvent
    {
        private readonly AutoPositionDownSettings _autoPositionDownSettings;

        public ApplyPositionDown(AutoPositionDownSettings autoPositionDownSettings) =>
            _autoPositionDownSettings = autoPositionDownSettings;

        protected internal override void OnDisablePhysics() =>
            _autoPositionDownSettings.ApplyOffset(SimulatedBody.GameObject);
    }
}
