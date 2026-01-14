#if UNITY_EDITOR
using System;
using UnityEngine.UIElements;
using VladislavTsurikov.CustomInspector.Editor.Core;

namespace VladislavTsurikov.CustomInspector.Editor.UIToolkit
{
    public abstract class UIToolkitFieldDrawer : FieldDrawer
    {
        public abstract VisualElement CreateField(string label, Type fieldType, object value, Action<object> onValueChanged);
    }
}
#endif
