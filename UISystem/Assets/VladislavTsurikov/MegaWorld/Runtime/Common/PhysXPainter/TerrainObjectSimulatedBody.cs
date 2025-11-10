using System.Collections.Generic;
using UnityEngine;
using VladislavTsurikov.PhysicsSimulator.Runtime;
using VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.Data;

namespace VladislavTsurikov.MegaWorld.Runtime.Common.PhysXPainter
{
    public class TerrainObjectSimulatedBody : SimulatedBody
    {
        internal TerrainObjectInstance TerrainObjectInstance;

        public TerrainObjectSimulatedBody(GameObject gameObject) : base(gameObject)
        {
        }

        public TerrainObjectSimulatedBody(GameObject gameObject,
            List<OnDisableSimulatedBodyEvent> onDisablePhysicsEvents) : base(gameObject, onDisablePhysicsEvents)
        {
        }
    }
}
