#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.UIElementsUtility.Runtime.Groups.Styles;

namespace VladislavTsurikov.UIElementsUtility.Editor.Groups.Styles
{
    [CustomEditor(typeof(StyleGroup))]
    public class StyleGroupEditor : UnityEditor.Editor
    {
        private StyleGroup _styleGroup => (StyleGroup)target;

        public override void OnInspectorGUI()
        {
            if (GUILayout.Button("Setup"))
            {
                Setup();
            }

            base.OnInspectorGUI();
        }

        private void Setup() => _styleGroup.Setup();
    }
}
#endif
