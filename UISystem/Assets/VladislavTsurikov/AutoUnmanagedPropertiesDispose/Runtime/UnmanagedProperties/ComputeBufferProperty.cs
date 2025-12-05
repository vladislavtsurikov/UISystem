using UnityEngine;

namespace VladislavTsurikov.AutoUnmanagedPropertiesDispose.Runtime
{
    public class ComputeBufferProperty : AutoDisposeProperty
    {
        public ComputeBuffer ComputeBuffer;

        public ComputeBufferProperty()
        {
        }

        public ComputeBufferProperty(ComputeBuffer computeBuffer) => ComputeBuffer = computeBuffer;

        public void ChangeComputeBuffer(ComputeBuffer computeBuffer)
        {
            InternalDisposeUnmanagedMemory();

            ComputeBuffer = computeBuffer;
        }

        protected override void InternalDisposeUnmanagedMemory()
        {
            if (ComputeBuffer != null)
            {
                ComputeBuffer.Release();
                ComputeBuffer = null;
            }
        }
    }
}
