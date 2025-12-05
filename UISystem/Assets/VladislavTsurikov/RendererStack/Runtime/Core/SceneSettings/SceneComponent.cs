using VladislavTsurikov.ComponentStack.Runtime.Core;

namespace VladislavTsurikov.RendererStack.Runtime.Core.SceneSettings
{
    public abstract class SceneComponent : Component
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
