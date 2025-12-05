#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.ComponentStack.Editor.Core;
using VladislavTsurikov.IMGUIUtility.Editor;
using VladislavTsurikov.IMGUIUtility.Editor.ElementStack.ReorderableList;

namespace VladislavTsurikov.RendererStack.Editor.TerrainObjectRenderer.GlobalSettings.ExtensionSystem.
    GameObjectConverter
{
    [ElementEditor(
        typeof(Runtime.TerrainObjectRenderer.GlobalSettings.ExtensionSystem.GameObjectConverter.GameObjectConverter))]
    [IMGUIUtility.Editor.ElementStack.ReorderableList.ContextMenu("Converter/GameObject Converter")]
    public class GameObjectConverterEditor : ReorderableListComponentEditor
    {
        public Runtime.TerrainObjectRenderer.GlobalSettings.ExtensionSystem.GameObjectConverter.GameObjectConverter
            GameObjectConverter;

        public override void OnEnable() =>
            GameObjectConverter =
                (Runtime.TerrainObjectRenderer.GlobalSettings.ExtensionSystem.GameObjectConverter.GameObjectConverter)
                Target;

        public override void OnGUI(Rect rect, int index)
        {
            EditorGUI.HelpBox(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                "Only added prefabs in Renderer are converted when using this button.", MessageType.Info);

            rect.y += CustomEditorGUI.SingleLineHeight;

            if (CustomEditorGUI.ClickButton(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                    "Convert GameObject To Terrain Object Renderer"))
            {
                Runtime.TerrainObjectRenderer.GlobalSettings.ExtensionSystem.GameObjectConverter.GameObjectConverter
                    .ConvertGameObjectToTerrainObjectRenderer();
            }

            rect.y += CustomEditorGUI.SingleLineHeight;

            if (CustomEditorGUI.ClickButton(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                    "Convert Terrain Object Renderer To GameObject"))
            {
                GameObjectConverter.ConvertTerrainObjectRendererToGameObject();
            }

            rect.y += CustomEditorGUI.SingleLineHeight;
        }

        public override float GetElementHeight(int index)
        {
            float height = 0;

            height += CustomEditorGUI.SingleLineHeight;
            height += CustomEditorGUI.SingleLineHeight;
            height += CustomEditorGUI.SingleLineHeight;

            return height;
        }
    }
}
#endif
