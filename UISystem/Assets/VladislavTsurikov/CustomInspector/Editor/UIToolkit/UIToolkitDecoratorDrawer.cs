#if UNITY_EDITOR
using UnityEngine.UIElements;
using VladislavTsurikov.CustomInspector.Editor.Core;

namespace VladislavTsurikov.CustomInspector.Editor.UIToolkit
{
    public abstract class UIToolkitDecoratorDrawer : DecoratorDrawer
    {
        public abstract VisualElement CreateElement();
    }
}
#endif
