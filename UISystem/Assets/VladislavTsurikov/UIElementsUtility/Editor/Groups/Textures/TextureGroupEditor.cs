#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.UIElementsUtility.Runtime.Groups.Textures;

namespace VladislavTsurikov.UIElementsUtility.Editor.Groups.Textures
{
    [CustomEditor(typeof(TextureGroup))]
    public class TextureGroupEditor : UnityEditor.Editor
    {
        private TextureGroup _textureGroup => (TextureGroup)target;

        public override void OnInspectorGUI()
        {
            if (GUILayout.Button("Setup"))
            {
                Setup();
            }

            base.OnInspectorGUI();
        }

        private void Setup() => _textureGroup.Setup();
    }
}
#endif
