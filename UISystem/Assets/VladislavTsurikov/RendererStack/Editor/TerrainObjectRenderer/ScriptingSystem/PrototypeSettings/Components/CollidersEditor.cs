#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.ComponentStack.Editor.Core;
using VladislavTsurikov.IMGUIUtility.Editor;
using VladislavTsurikov.RendererStack.Editor.Core.PrototypeRendererSystem.PrototypeSettings;
using VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.ScriptingSystem.PrototypeSettings;

namespace VladislavTsurikov.RendererStack.Editor.TerrainObjectRenderer.ScriptingSystem.PrototypeSettings
{
    [ElementEditor(typeof(Colliders))]
    public class CollidersEditor : PrototypeComponentEditor
    {
        private Colliders _colliders;

        public override void OnEnable() => _colliders = (Colliders)Target;

        public override void OnGUI(Rect rect, int index) =>
            _colliders.MaxDistance = CustomEditorGUI.FloatField(
                new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), new GUIContent("Max Distance"),
                _colliders.MaxDistance);

        public override float GetElementHeight(int index) => CustomEditorGUI.SingleLineHeight;
    }
}
#endif
