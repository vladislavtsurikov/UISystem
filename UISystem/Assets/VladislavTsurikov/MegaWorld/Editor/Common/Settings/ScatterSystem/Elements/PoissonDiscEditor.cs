#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.ComponentStack.Editor.Core;
using VladislavTsurikov.IMGUIUtility.Editor.ElementStack.ReorderableList;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.ScatterSystem;

namespace VladislavTsurikov.MegaWorld.Editor.Common.Settings.ScatterSystem
{
    [ElementEditor(typeof(PoissonDisc))]
    public class PoissonDiscEditor : ReorderableListComponentEditor
    {
        private PoissonDisc _poissonDisc;

        public override void OnEnable() => _poissonDisc = (PoissonDisc)Target;

        public override void OnGUI(Rect rect, int index)
        {
            _poissonDisc.PoissonDiscSize = EditorGUI.FloatField(
                new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                new GUIContent("Poisson Disc Size"), _poissonDisc.PoissonDiscSize);
            rect.y += EditorGUIUtility.singleLineHeight;
        }

        public override float GetElementHeight(int index)
        {
            var height = EditorGUIUtility.singleLineHeight;

            height += EditorGUIUtility.singleLineHeight;

            return height;
        }
    }
}
#endif
