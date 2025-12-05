using System.Collections.Generic;
using UnityEngine;
using VladislavTsurikov.BVH.Runtime;
using VladislavTsurikov.Math.Runtime;

namespace VladislavTsurikov.ColliderSystem.Runtime
{
    public class MeshTree
    {
        private readonly Mesh _mesh;
        private readonly BVHTree<BVHNodeAABB<NodeData>, NodeData> _tree = new();
        private bool _isReady;

        public MeshTree(Mesh mesh) => _mesh = mesh;

        public MeshRayHit Raycast(Ray ray, Matrix4x4 transformMtx)
        {
            if (!_isReady)
            {
                Build();
            }

            List<BVHNodeRayHit<NodeData>> nodeHits = _tree.RaycastAll(transformMtx.inverse.TransformRay(ray), false);
            if (nodeHits.Count != 0)
            {
                var minT = float.MaxValue;
                Vector3 hitTriNormal = Vector3.zero;
                var hitTriIndex = -1;
                foreach (BVHNodeRayHit<NodeData> nodeHit in nodeHits)
                {
                    var triangleIndex = nodeHit.HitNode.Data.TriangleIndex;
                    Vector3[] triangleVerts = _mesh.GetTriangleVerts(triangleIndex);

                    triangleVerts[0] = transformMtx.MultiplyPoint(triangleVerts[0]);
                    triangleVerts[1] = transformMtx.MultiplyPoint(triangleVerts[1]);
                    triangleVerts[2] = transformMtx.MultiplyPoint(triangleVerts[2]);

                    Vector3 e0 = triangleVerts[1] - triangleVerts[0];
                    Vector3 e1 = triangleVerts[2] - triangleVerts[0];
                    Vector3 triNormal = Vector3.Cross(e0, e1).normalized;

                    float t;
                    if (TriangleMath.Raycast(ray, out t, triangleVerts[0], triangleVerts[1], triangleVerts[2],
                            triNormal))
                    {
                        if (t < minT)
                        {
                            minT = t;
                            hitTriNormal = triNormal;
                            hitTriIndex = triangleIndex;
                        }
                    }
                }

                if (hitTriIndex >= 0)
                {
                    return new MeshRayHit(ray, minT, hitTriIndex, hitTriNormal);
                }
            }

            return null;
        }

        public void Build()
        {
            if (_isReady)
            {
                return;
            }

            Vector3[] meshVerts = _mesh.Vertices;
            for (var triIndex = 0; triIndex < _mesh.NumTriangles; ++triIndex)
            {
                MeshTriangle triangle = _mesh.GetTriangle(triIndex);

                Vector3 min = meshVerts[triangle.Index0];
                min = Vector3.Min(min, meshVerts[triangle.Index1]);
                min = Vector3.Min(min, meshVerts[triangle.Index2]);

                Vector3 max = meshVerts[triangle.Index0];
                max = Vector3.Max(max, meshVerts[triangle.Index1]);
                max = Vector3.Max(max, meshVerts[triangle.Index2]);

                var node = new BVHNodeAABB<NodeData>(new NodeData { TriangleIndex = triIndex })
                {
                    Size = max - min, Position = (min + max) * 0.5f
                };

                _tree.InsertLeafNode(node);
            }

            _isReady = true;
        }

        private class NodeData
        {
            public int TriangleIndex;
        }

#if UNITY_EDITOR
        public void DrawRaycast(Ray ray, Matrix4x4 transformMtx, Color lineColor)
        {
            if (!_isReady)
            {
                Build();
            }

            _tree.DrawRaycast(transformMtx.inverse.TransformRay(ray), transformMtx, lineColor);
        }

        public void DrawAllCells(Matrix4x4 transformMtx, Color lineColor)
        {
            if (!_isReady)
            {
                Build();
            }

            _tree.DrawAllCells(transformMtx, lineColor);
        }
#endif
    }
}
