#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.ComponentStack.Editor.Core;
using VladislavTsurikov.IMGUIUtility.Editor;
using VladislavTsurikov.RendererStack.Editor.Core.PrototypeRendererSystem.PrototypeSettings;
using VladislavTsurikov.RendererStack.Runtime.Common.PrototypeSettings;

namespace VladislavTsurikov.RendererStack.Editor.Common.PrototypeSettings
{
    [ElementEditor(typeof(DistanceCulling))]
    public class DistanceCullingEditor : PrototypeComponentEditor
    {
        private DistanceCulling _distanceCulling;

        public GUIContent DistanceRandomOffset =
            new("Distance Random Offset", "Decreases Max Distance randomly with offset.");

        public GUIContent MaxDistance = new("Max Distance",
            "Defines maximum distance from the camera within which this prototype will be rendered.");

        public override void OnEnable() => _distanceCulling = (DistanceCulling)Target;

        public override void OnGUI(Rect rect, int index)
        {
            _distanceCulling.MaxDistance = CustomEditorGUI.FloatField(
                new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), MaxDistance,
                _distanceCulling.MaxDistance);
            rect.y += CustomEditorGUI.SingleLineHeight;
            _distanceCulling.DistanceRandomOffset = CustomEditorGUI.FloatField(
                new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), DistanceRandomOffset,
                _distanceCulling.DistanceRandomOffset);
            rect.y += CustomEditorGUI.SingleLineHeight;
        }

        public override float GetElementHeight(int index)
        {
            float height = 0;

            height += CustomEditorGUI.SingleLineHeight;
            height += CustomEditorGUI.SingleLineHeight;

            return height;
        }
    }
}
#endif
