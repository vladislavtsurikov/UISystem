#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.UIElementsUtility.Runtime.Groups.Layouts;

namespace VladislavTsurikov.UIElementsUtility.Editor.Groups.Layouts
{
    [CustomEditor(typeof(LayoutGroup))]
    public class LayoutGroupEditor : UnityEditor.Editor
    {
        private LayoutGroup _layoutGroup => (LayoutGroup)target;

        public override void OnInspectorGUI()
        {
            if (GUILayout.Button("Setup"))
            {
                Setup();
            }

            base.OnInspectorGUI();
        }

        private void Setup() => _layoutGroup.Setup();
    }
}
#endif
