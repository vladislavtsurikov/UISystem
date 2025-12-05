#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.UIElementsUtility.Runtime.Groups.Fonts;

namespace VladislavTsurikov.UIElementsUtility.Editor.Groups.Fonts
{
    [CustomEditor(typeof(FontFamily))]
    public class FontFamilyEditor : UnityEditor.Editor
    {
        private FontFamily _fontFamily => (FontFamily)target;

        public override void OnInspectorGUI()
        {
            if (GUILayout.Button("Setup"))
            {
                Setup();
            }

            base.OnInspectorGUI();
        }

        private void Setup() => _fontFamily.Setup();
    }
}
#endif
