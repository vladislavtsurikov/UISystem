using Unity.Collections;

namespace VladislavTsurikov.AutoUnmanagedPropertiesDispose.Runtime
{
    public class NativeArrayProperty<T> : AutoDisposeProperty where T : unmanaged
    {
        public NativeArray<T> NativeArray;

        public NativeArrayProperty()
        {
        }

        public NativeArrayProperty(NativeArray<T> nativeArray) => NativeArray = nativeArray;

        public void ChangeNativeArray(NativeArray<T> nativeArray)
        {
            InternalDisposeUnmanagedMemory();

            NativeArray = nativeArray;
        }

        protected override void InternalDisposeUnmanagedMemory()
        {
            if (NativeArray.IsCreated)
            {
                NativeArray.Dispose();
            }
        }
    }
}
