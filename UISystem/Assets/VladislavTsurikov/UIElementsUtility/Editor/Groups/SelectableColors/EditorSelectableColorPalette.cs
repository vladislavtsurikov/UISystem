#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace VladislavTsurikov.UIElementsUtility.Editor.Groups.SelectableColors
{
    [CustomEditor(typeof(EditorSelectableColorPalette))]
    public class EditorSelectableColorPaletteEditor : UnityEditor.Editor
    {
        private EditorSelectableColorPalette _editorSelectableColorPalette => (EditorSelectableColorPalette)target;

        public override void OnInspectorGUI()
        {
            if (GUILayout.Button("Setup"))
            {
                Setup();
            }

            base.OnInspectorGUI();
        }

        private void Setup() => _editorSelectableColorPalette.Setup();
    }
}
#endif
