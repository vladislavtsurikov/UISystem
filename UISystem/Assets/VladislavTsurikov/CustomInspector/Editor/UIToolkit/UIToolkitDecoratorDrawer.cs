#if UNITY_EDITOR
using System.Reflection;
using UnityEngine.UIElements;
using VladislavTsurikov.CustomInspector.Editor.Core;

namespace VladislavTsurikov.CustomInspector.Editor.UIToolkit
{
    public abstract class UIToolkitDecoratorDrawer : DecoratorDrawer
    {
        public abstract VisualElement CreateElement(FieldInfo field, object target);
    }
}
#endif
