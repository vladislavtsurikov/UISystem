#if UNITY_EDITOR
using System.Collections.Generic;
using VladislavTsurikov.UIElementsUtility.Runtime;
using VladislavTsurikov.UIElementsUtility.Runtime.Core;

namespace VladislavTsurikov.UIElementsUtility.Editor.Core
{
    public abstract class DataGroupAPIGenerator<T, N> where T : DataGroup<T, N>
    {
        protected static string TargetFilePath => $"{RuntimePath.Path}/API";

        public void Run() => Generate(DataGroup<T, N>.AllInstances);

        protected abstract void Generate(List<T> groups);
    }
}
#endif
