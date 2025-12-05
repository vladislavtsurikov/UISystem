namespace VladislavTsurikov.AutoUnmanagedPropertiesDispose.Runtime
{
    /*public class AutoDisposeManager : ScriptableSingleton<AutoDisposeManager>
    {
        private List<AutoDisposeProperty> s_autoDisposePropertyList = new List<AutoDisposeProperty>();

        private void OnDisable()
        {
            DisposeAllUnmanagedMemory();
        }

        public void RemoveAutoDisposeProperty(AutoDisposeProperty autoDisposeProperty)
        {
            s_autoDisposePropertyList.Remove(autoDisposeProperty);
        }

        public void AddUnmanagedProperties(AutoDisposeProperty unmanagedProperties)
        {
            s_autoDisposePropertyList.Add(unmanagedProperties);
        }

        public void DisposeAllUnmanagedMemory()
        {
            Debug.Log(s_autoDisposePropertyList.Count);
            for (int i = s_autoDisposePropertyList.Count -1; i >= 0; i--)
            {
                if (s_autoDisposePropertyList[i] == null)
                {
                    s_autoDisposePropertyList.RemoveAt(i);
                    continue;
                }
                s_autoDisposePropertyList[i].DisposeUnmanagedMemory();
            }
        }
    }*/
}
