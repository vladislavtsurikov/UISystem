#if UNITY_EDITOR
using UnityEngine.UIElements;
using VladislavTsurikov.CustomInspector.Editor.Core;

namespace VladislavTsurikov.CustomInspector.Editor.UIToolkit
{
    internal sealed class UIToolkitFieldPresentationScope : FieldPresentationScope
    {
        public UIToolkitFieldPresentationScope(FieldState state, FieldStyle style, VisualElement fieldElement)
            : base(state, style, fieldElement)
        {
            if (fieldElement == null)
            {
                return;
            }

            if (!state.IsEnabled || state.IsReadOnly)
            {
                fieldElement.SetEnabled(false);
            }

            if (style.Color.HasValue)
            {
                fieldElement.style.backgroundColor = new StyleColor(style.Color.Value);
            }
        }

        public override void Dispose()
        {
        }
    }
}
#endif
