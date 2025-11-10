using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;
using VladislavTsurikov.AutoUnmanagedPropertiesDispose.Runtime;
using VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.Data;
using PrototypeRenderData =
    VladislavTsurikov.RendererStack.Runtime.Core.SceneSettings.Camera.CameraTemporarySettingsSystem.ObjectCameraRender.
    PrototypeRenderData;

namespace VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.GPUInstancedIndirect
{
    public static class MergeInstancedIndirectBuffersID
    {
        private static readonly ComputeBufferProperty _dummyComputeBuffer = new();

        private static ComputeShader _mergeBufferShader;

        private static readonly int[] _count = new int[16];
        private static readonly int[] _instances = new int[16];

        private static int[] _addInstances;
        private static int[] _maxNumberCellsKernel;
        private static int[] _threads;

        private static int _addInstances1;
        private static int _addInstances2;
        private static int _addInstances4;
        private static int _addInstances8;
        private static int _addInstances16;

        public static void Setup()
        {
            _dummyComputeBuffer.ChangeComputeBuffer(new ComputeBuffer(1, 16 * 4 + 16, ComputeBufferType.Default));

            _mergeBufferShader = (ComputeShader)Resources.Load("RendererStackMergeInstancedIndirectBuffers");

            for (var index = 0; index < _count.Length; ++index)
            {
                _count[index] = Shader.PropertyToID("Count" + index);
            }

            for (var index = 0; index < _instances.Length; ++index)
            {
                _instances[index] = Shader.PropertyToID("Instances" + index);
            }

            _maxNumberCellsKernel = new[] { 1, 2, 4, 8, 16, 32 };

            _threads = new[] { 64, 64, 64, 64, 64, 128 };

            _addInstances1 = _mergeBufferShader.FindKernel("AddInstances1");
            _addInstances2 = _mergeBufferShader.FindKernel("AddInstances2");
            _addInstances4 = _mergeBufferShader.FindKernel("AddInstances4");
            _addInstances8 = _mergeBufferShader.FindKernel("AddInstances8");
            _addInstances16 = _mergeBufferShader.FindKernel("AddInstances16");
            _addInstances = new[]
            {
                _addInstances1, _addInstances2, _addInstances4, _addInstances4, _addInstances8, _addInstances8,
                _addInstances8, _addInstances8, _addInstances16, _addInstances16, _addInstances16, _addInstances16,
                _addInstances16, _addInstances16, _addInstances16, _addInstances16
            };
        }

        public static void MergeBuffer(List<Cell> instancedIndirectCellList, PrototypeRenderData prototypeRenderData,
            int protoID)
        {
            Profiler.BeginSample("Merge Buffer");

            prototypeRenderData.MergeBuffer.ComputeBuffer.SetCounterValue(0);

            var batchAddList = new List<BatchAdd>();
            for (var cellIndex = 0; cellIndex < instancedIndirectCellList.Count; cellIndex++)
            {
                Cell cell = instancedIndirectCellList[cellIndex];

                var instanceCount = cell.PrototypeRenderDataStack.GetPrototypeRenderData(protoID).InstanceList.Count;

                var batch = new BatchAdd
                {
                    Instances = cell.PrototypeRenderDataStack.GetPrototypeRenderData(protoID).TemporaryInstances
                        .ComputeBufferRenderData.ComputeBufferProperty.ComputeBuffer,
                    Count = instanceCount
                };

                batchAddList.Add(batch);

                if (batchAddList.Count == 16)
                {
                    MergeBuffer(batchAddList, prototypeRenderData);
                    batchAddList.Clear();
                }
            }

            if (batchAddList.Count != 0)
            {
                MergeBuffer(batchAddList, prototypeRenderData);
            }

            Profiler.EndSample();
        }

        private static void MergeBuffer(IReadOnlyList<BatchAdd> batches, PrototypeRenderData prototypeRenderData)
        {
            var addInstance = _addInstances[batches.Count - 1];
            var instanceCount = 0;
            for (var index = 0; index < batches.Count; index++)
            {
                instanceCount = Mathf.Max(instanceCount, batches[index].Count);

                _mergeBufferShader.SetInt(_count[index], batches[index].Count);
                _mergeBufferShader.SetBuffer(addInstance, _instances[index], batches[index].Instances);
            }

            for (var length = batches.Count; length < _maxNumberCellsKernel[addInstance]; length++)
            {
                _mergeBufferShader.SetInt(_count[length], 0);
                _mergeBufferShader.SetBuffer(addInstance, _instances[length], _dummyComputeBuffer.ComputeBuffer);
            }

            var threadGroups = Mathf.CeilToInt((float)instanceCount / _threads[addInstance]);
            if (threadGroups == 0)
            {
                return;
            }

            _mergeBufferShader.SetBuffer(addInstance, GPUFrustumCullingID.MergeBufferID,
                prototypeRenderData.MergeBuffer.ComputeBuffer);

            _mergeBufferShader.Dispatch(addInstance, threadGroups, 1, 1);
        }

        public static void Dispose() => _dummyComputeBuffer.DisposeUnmanagedMemory();

        private struct BatchAdd
        {
            public ComputeBuffer Instances;
            public int Count;
        }
    }
}
