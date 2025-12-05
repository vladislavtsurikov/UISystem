#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using VladislavTsurikov.Undo.Editor;

namespace VladislavTsurikov.MegaWorld.Runtime.Common.PhysXPainter.Undo
{
    public class CreatedTerrainObjectSimulatedBody : UndoRecord
    {
        private readonly List<TerrainObjectSimulatedBody> _instanceList = new();

        public CreatedTerrainObjectSimulatedBody(TerrainObjectSimulatedBody obj) => _instanceList.Add(obj);

        public override void Merge(UndoRecord record)
        {
            if (record is not CreatedTerrainObjectSimulatedBody undo)
            {
                return;
            }

            _instanceList.AddRange(undo._instanceList);
        }

        public override void Undo()
        {
            foreach (TerrainObjectSimulatedBody obj in _instanceList)
            {
                if (obj.TerrainObjectInstance != null)
                {
                    obj.TerrainObjectInstance.Destroy();
                }
                else
                {
                    Object.DestroyImmediate(obj.GameObject);
                }
            }
        }
    }
}
#endif
