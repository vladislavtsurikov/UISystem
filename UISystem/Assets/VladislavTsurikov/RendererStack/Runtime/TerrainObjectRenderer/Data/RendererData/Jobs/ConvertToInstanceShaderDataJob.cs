using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Mathematics;

namespace VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.Data.RendererData
{
    [BurstCompile(CompileSynchronously = true)]
    public struct ConvertToInstanceShaderDataJob : IJobParallelFor
    {
        [ReadOnly]
        public NativeArray<Instance> InstanceArray;

        [NativeDisableUnsafePtrRestriction]
        public unsafe void* InstanceShaderDataPtr;

        public unsafe void Execute(int index)
        {
            ref InstanceShaderData instanceShaderData =
                ref UnsafeUtility.ArrayElementAsRef<InstanceShaderData>(InstanceShaderDataPtr, index);

            Instance instance = InstanceArray[index];

            instanceShaderData.SetMatrix(float4x4.TRS(instance.Position, instance.Rotation, instance.Scale));
        }
    }
}
