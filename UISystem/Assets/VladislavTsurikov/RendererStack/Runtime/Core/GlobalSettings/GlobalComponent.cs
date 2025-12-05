using VladislavTsurikov.ComponentStack.Runtime.Core;

namespace VladislavTsurikov.RendererStack.Runtime.Core.GlobalSettings
{
    public abstract class GlobalComponent : Component
    {
#if UNITY_EDITOR
        public virtual void OnSelectedDrawGizmos()
        {
        }

        public virtual void OnDrawGizmos()
        {
        }
#endif
    }
}
