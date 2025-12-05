using UnityEngine;
using VladislavTsurikov.ColliderSystem.Runtime;
using VladislavTsurikov.UnityUtility.Runtime;

namespace VladislavTsurikov.MegaWorld.Runtime.Common.Area
{
    public class BoxArea : Area
    {
        public readonly RayHit RayHit;
        public readonly Terrain TerrainUnder;

        public BoxArea(RayHit rayHit, float size) : base(rayHit.Point, size)
        {
            RayHit = rayHit;
            TerrainUnder = UnityTerrainUtility.GetTerrain(rayHit.Point);
        }

        public float BoxSize => Size.x;
        public float Radius => BoxSize / 2;
    }
}
