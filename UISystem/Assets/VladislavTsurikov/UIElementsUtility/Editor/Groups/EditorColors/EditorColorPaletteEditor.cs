#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace VladislavTsurikov.UIElementsUtility.Editor.Groups.EditorColors
{
    [CustomEditor(typeof(EditorColorPalette))]
    public class EditorColorPaletteEditor : UnityEditor.Editor
    {
        private EditorColorPalette _editorColorPalette => (EditorColorPalette)target;

        public override void OnInspectorGUI()
        {
            if (GUILayout.Button("Setup"))
            {
                Setup();
            }

            base.OnInspectorGUI();
        }

        private void Setup() => _editorColorPalette.Setup();
    }
}
#endif
