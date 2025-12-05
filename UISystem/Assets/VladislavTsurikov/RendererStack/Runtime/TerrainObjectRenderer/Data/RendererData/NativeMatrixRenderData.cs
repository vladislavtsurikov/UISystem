using System.Collections.Generic;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using VladislavTsurikov.AutoUnmanagedPropertiesDispose.Runtime;
using VladislavTsurikov.UnityUtility.Runtime;

namespace VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.Data.RendererData
{
    public struct InstanceShaderData
    {
        public float4x4 Matrix;
        public float4x4 InverseMatrix;
        public float4 LODControl;

        public void SetMatrix(float4x4 matrix)
        {
            Matrix = matrix;
            InverseMatrix = math.inverse(Matrix);
            LODControl = new Vector4();
        }
    }

    public class NativeRenderData
    {
        public NativeArrayProperty<InstanceShaderData> InstanceShaderDataList = new();

        public unsafe void ConvertPersistentDataToRenderData(List<Instance> instances)
        {
            InstanceShaderDataList.ChangeNativeArray(new NativeArray<InstanceShaderData>());

            if (instances.Count != 0)
            {
                var nativeItemArray = new NativeArray<Instance>(instances.Count, Allocator.Persistent);
                nativeItemArray.CopyFromFast(instances);

                InstanceShaderDataList.ChangeNativeArray(
                    new NativeArray<InstanceShaderData>(instances.Count, Allocator.Persistent));

                var loadPersistentStorageToMatrixJob =
                    new ConvertToInstanceShaderDataJob
                    {
                        InstanceArray = nativeItemArray,
                        InstanceShaderDataPtr = InstanceShaderDataList.NativeArray.GetUnsafePtr()
                    };

                loadPersistentStorageToMatrixJob.Schedule(instances.Count, 64).Complete();

                nativeItemArray.Dispose();
            }
        }
    }
}
