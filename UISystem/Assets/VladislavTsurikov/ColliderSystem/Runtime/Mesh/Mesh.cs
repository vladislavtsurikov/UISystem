using System.Collections.Generic;
using UnityEngine;
using VladislavTsurikov.Math.Runtime;

namespace VladislavTsurikov.ColliderSystem.Runtime
{
    public class Mesh
    {
        private readonly MeshTree _tree;
        private readonly Vector3[] _vertices;
        private readonly int[] _vertIndices;

        public Mesh(UnityEngine.Mesh unityMesh)
        {
            UnityMesh = unityMesh;
            _vertices = UnityMesh.vertices;
            _vertIndices = UnityMesh.triangles;
            NumTriangles = _vertIndices.Length / 3;
            _tree = new MeshTree(this);
        }

        public UnityEngine.Mesh UnityMesh { get; }

        public int NumTriangles { get; }

        public Vector3[] Vertices => _vertices.Clone() as Vector3[];

        public Vector3[] GetTriangleVerts(int triangleIndex)
        {
            var baseIndex = triangleIndex * 3;
            return new[]
            {
                _vertices[_vertIndices[baseIndex]], _vertices[_vertIndices[baseIndex + 1]],
                _vertices[_vertIndices[baseIndex + 2]]
            };
        }

        public List<Vector3> GetTriangleVerts(int triangleIndex, Matrix4x4 transformMatrix)
        {
            var baseIndex = triangleIndex * 3;
            return transformMatrix.TransformPoints(new List<Vector3>
            {
                _vertices[_vertIndices[baseIndex]],
                _vertices[_vertIndices[baseIndex + 1]],
                _vertices[_vertIndices[baseIndex + 2]]
            });
        }

        public MeshTriangle GetTriangle(int triangleIndex)
        {
            var baseIndex = triangleIndex * 3;
            return new MeshTriangle(_vertIndices[baseIndex], _vertIndices[baseIndex + 1], _vertIndices[baseIndex + 2]);
        }

        public MeshRayHit Raycast(Ray ray, Matrix4x4 transformMtx) => _tree.Raycast(ray, transformMtx);

#if UNITY_EDITOR
        public void DrawRaycast(Ray ray, Matrix4x4 transformMtx, Color lineColor) =>
            _tree.DrawRaycast(ray, transformMtx, lineColor);

        public void DrawAllCells(Matrix4x4 transformMtx, Color lineColor) =>
            _tree.DrawAllCells(transformMtx, lineColor);
#endif
    }
}
