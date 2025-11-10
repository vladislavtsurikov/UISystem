#if UNITY_EDITOR
using UnityEngine;
using VladislavTsurikov.IMGUIUtility.Editor.ElementStack;
using Renderer = VladislavTsurikov.RendererStack.Runtime.Core.RendererSystem.Renderer;

namespace VladislavTsurikov.RendererStack.Editor.Core.RendererSystem
{
    public class RendererEditor : IMGUIElementEditor
    {
        private Vector2 _windowScrollPos;
        public Renderer RendererTarget => (Renderer)Target;

        public virtual RendererMenu GetRendererMenu() => null;
    }
}
#endif
