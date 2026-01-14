#if UNITY_EDITOR
using UnityEngine;
using VladislavTsurikov.CustomInspector.Editor.Core;

namespace VladislavTsurikov.CustomInspector.Editor.IMGUI
{
    public abstract class IMGUIDecoratorDrawer : DecoratorDrawer
    {
        public abstract void Draw(Rect rect);
        public abstract float GetHeight();
    }
}
#endif
