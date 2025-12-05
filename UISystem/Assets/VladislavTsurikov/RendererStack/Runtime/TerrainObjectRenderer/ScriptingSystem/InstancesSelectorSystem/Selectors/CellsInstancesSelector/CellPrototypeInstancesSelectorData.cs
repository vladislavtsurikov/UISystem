using System.Collections.Generic;
using UnityEngine;
using VladislavTsurikov.Math.Runtime;
using VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.Data.ColliderSystem;

namespace VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.ScriptingSystem.InstancesSelectorSystem.
    CellsInstancesSelector
{
    public class CellPrototypeInstancesSelectorData : PrototypeInstancesSelectorData
    {
        private Vector3 _lastCenter;
        private float _lastDistance;
        public List<ColliderCell> LastColliderCellList = new();

        public CellPrototypeInstancesSelectorData(object obj) : base(obj)
        {
        }

        public override bool IsNeedUpdate(Sphere sphere)
        {
            var needsUpdate = false;

            var distance = Vector3.Distance(_lastCenter, sphere.Center);
            if (distance > TerrainObjectRendererCollider.ConstCellSize ||
                Mathf.Abs(_lastDistance - sphere.Radius) > 0.1f)
            {
                needsUpdate = true;
                _lastCenter = sphere.Center;
                _lastDistance = sphere.Radius;
            }

            return needsUpdate;
        }
    }
}
