#if UNITY_EDITOR
using System.Reflection;
using UnityEngine;
using VladislavTsurikov.CustomInspector.Editor.Core;

namespace VladislavTsurikov.CustomInspector.Editor.IMGUI
{
    public abstract class IMGUIDecoratorDrawer : DecoratorDrawer
    {
        public abstract void Draw(Rect rect, FieldInfo field, object target);
        public abstract float GetHeight(FieldInfo field, object target);
    }
}
#endif
