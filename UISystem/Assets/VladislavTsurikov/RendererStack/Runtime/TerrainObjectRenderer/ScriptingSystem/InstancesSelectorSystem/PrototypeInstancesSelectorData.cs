using VladislavTsurikov.Math.Runtime;

namespace VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.ScriptingSystem.InstancesSelectorSystem
{
    public class PrototypeInstancesSelectorData
    {
        public readonly object Object;

        protected PrototypeInstancesSelectorData(object obj) => Object = obj;

        public virtual bool IsNeedUpdate(Sphere sphere) => true;
    }
}
