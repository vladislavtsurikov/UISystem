using System.Runtime.InteropServices;
using Unity.Collections;
using UnityEngine;
using VladislavTsurikov.AutoUnmanagedPropertiesDispose.Runtime;

namespace VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.Data.RendererData
{
    public class ComputeBufferRenderData
    {
        public ComputeBufferProperty ComputeBufferProperty = new();

        public void ConvertPersistentDataToRenderData(NativeArray<InstanceShaderData> instanceShaderDataArray)
        {
            if (instanceShaderDataArray.Length != 0)
            {
                var length = instanceShaderDataArray.Length;

                ComputeBufferProperty.ChangeComputeBuffer(new ComputeBuffer(length,
                    Marshal.SizeOf(typeof(InstanceShaderData))));
                ComputeBufferProperty.ComputeBuffer.SetData(instanceShaderDataArray);
            }
        }
    }
}
