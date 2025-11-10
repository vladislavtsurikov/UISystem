#if UNITY_EDITOR
using UnityEditor;
using VladislavTsurikov.RendererStack.Runtime.Core.RendererSystem;

namespace VladislavTsurikov.RendererStack.Editor.Core.RendererSystem
{
    public abstract class RendererMenu
    {
        public abstract void ShowGenericMenu(GenericMenu menu, Renderer renderer);
    }
}
#endif
