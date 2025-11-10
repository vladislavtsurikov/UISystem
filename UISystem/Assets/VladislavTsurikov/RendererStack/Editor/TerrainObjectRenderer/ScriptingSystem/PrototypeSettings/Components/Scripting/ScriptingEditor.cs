#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.ComponentStack.Editor.Core;
using VladislavTsurikov.ComponentStack.Runtime.Core;
using VladislavTsurikov.IMGUIUtility.Editor;
using VladislavTsurikov.RendererStack.Editor.Core.PrototypeRendererSystem.PrototypeSettings;
using VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.ScriptingSystem.PrototypeSettings;

namespace VladislavTsurikov.RendererStack.Editor.TerrainObjectRenderer.ScriptingSystem.PrototypeSettings.Scripting
{
    [ElementEditor(typeof(Runtime.TerrainObjectRenderer.ScriptingSystem.PrototypeSettings.Scripting.Scripting))]
    public class ScriptingEditor : PrototypeComponentEditor
    {
        private Runtime.TerrainObjectRenderer.ScriptingSystem.PrototypeSettings.Scripting.Scripting _scripting;
        private ScriptStackEditor _settingsStackEditor;

        public override void OnEnable()
        {
            _scripting = (Runtime.TerrainObjectRenderer.ScriptingSystem.PrototypeSettings.Scripting.Scripting)Target;
            _settingsStackEditor = new ScriptStackEditor(_scripting.ScriptStack);
        }

        public override void OnGUI(Rect rect, int index)
        {
            var colliders = (Colliders)Prototype.GetSettings(typeof(Colliders));

            if (!colliders.IsValid())
            {
                _scripting.MaxDistance = CustomEditorGUI.FloatField(
                    new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                    new GUIContent("Max Distance"), _scripting.MaxDistance);
                rect.y += CustomEditorGUI.SingleLineHeight;
            }

            EditorGUI.BeginDisabledGroup(Application.isPlaying);
            _settingsStackEditor.OnGUI(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight));
            EditorGUI.EndDisabledGroup();
        }

        public override float GetElementHeight(int index)
        {
            float height = 0;

            var colliders = (Colliders)Prototype.GetSettings(typeof(Colliders));

            if (!colliders.IsValid())
            {
                height += CustomEditorGUI.SingleLineHeight;
            }

            height += _settingsStackEditor.GetElementStackHeight();

            return height;
        }
    }
}
#endif
