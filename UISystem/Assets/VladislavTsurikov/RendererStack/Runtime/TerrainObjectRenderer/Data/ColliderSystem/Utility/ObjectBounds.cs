using UnityEngine;
using VladislavTsurikov.Math.Runtime;

namespace VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.Data.ColliderSystem
{
    public static class ObjectBounds
    {
        public static OBB CalcWorldOBB(Mesh mesh, Matrix4x4 transformMatrix)
        {
            AABB modelAABB = CalcModelAABB(mesh);
            if (!modelAABB.IsValid)
            {
                return OBB.GetInvalid();
            }

            return new OBB(modelAABB, transformMatrix);
        }

        public static AABB CalcWorldAABB(Mesh mesh, Matrix4x4 transformMatrix)
        {
            AABB modelAABB = CalcModelAABB(mesh);
            if (!modelAABB.IsValid)
            {
                return modelAABB;
            }

            modelAABB.Transform(transformMatrix);
            return modelAABB;
        }

        private static AABB CalcModelAABB(Mesh mesh)
        {
            if (mesh != null)
            {
                return new AABB(mesh.bounds);
            }

            return new AABB(Vector3.zero, Vector3.one);
        }
    }
}
