namespace VladislavTsurikov.AutoUnmanagedPropertiesDispose.Runtime
{
    /*public unsafe class NativeListProperty<T> : AutoDisposeProperty where T : unmanaged
    {
        public NativeList<T> NativeList;

        public NativeListProperty(NativeList<T> nativeList)
        {
            NativeList = nativeList;
        }

        public void ChangeNativeList(NativeList<T> nativeList)
        {
            InternalDisposeUnmanagedMemory();

            NativeList = nativeList;
        }

        protected override void InternalDisposeUnmanagedMemory()
        {
            if(NativeList.IsCreated)
            {
                NativeList.Dispose();
            }
        }
    }*/
}
