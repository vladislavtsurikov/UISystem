#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.CustomInspector.Editor.Core;

namespace VladislavTsurikov.CustomInspector.Editor.IMGUI
{
    internal sealed class IMGUIFieldPresentationScope : FieldPresentationScope
    {
        private readonly Color _originalColor;
        private readonly bool _hasColor;

        public IMGUIFieldPresentationScope(FieldState state, FieldStyle style)
            : base(state, style, null)
        {
            _originalColor = GUI.color;
            _hasColor = style.Color.HasValue;

            if (_hasColor)
            {
                GUI.color = style.Color.Value;
            }

            EditorGUI.BeginDisabledGroup(!state.IsEnabled || state.IsReadOnly);
        }

        public override void Dispose()
        {
            EditorGUI.EndDisabledGroup();

            if (_hasColor)
            {
                GUI.color = _originalColor;
            }
        }
    }
}
#endif
