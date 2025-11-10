#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.ComponentStack.Editor.Core;
using VladislavTsurikov.IMGUIUtility.Editor;
using VladislavTsurikov.IMGUIUtility.Editor.ElementStack.ReorderableList;

namespace VladislavTsurikov.RendererStack.Editor.TerrainObjectRenderer.GlobalSettings.ExtensionSystem.
    UnityTerrainTreeConverter
{
    [ElementEditor(
        typeof(Runtime.TerrainObjectRenderer.GlobalSettings.ExtensionSystem.UnityTerrainTreeConverter.
            UnityTerrainTreeConverter))]
    [IMGUIUtility.Editor.ElementStack.ReorderableList.ContextMenu("Converter/UnityTerrainTree Converter")]
    public class UnityTerrainTreeConverterEditor : ReorderableListComponentEditor
    {
        public Runtime.TerrainObjectRenderer.GlobalSettings.ExtensionSystem.UnityTerrainTreeConverter.
            UnityTerrainTreeConverter UnityTerrainTreeConverter;

        public override void OnEnable() =>
            UnityTerrainTreeConverter =
                (Runtime.TerrainObjectRenderer.GlobalSettings.ExtensionSystem.UnityTerrainTreeConverter.
                    UnityTerrainTreeConverter)Target;

        public override void OnGUI(Rect rect, int index)
        {
            if (CustomEditorGUI.ClickButton(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                    "Remove All Unity Terrain Prototypes From Terrains", ButtonStyle.Remove))
            {
                if (EditorUtility.DisplayDialog("WARNING!",
                        "Are you sure you want to remove all Unity Terrain Tree Resources from the scene?",
                        "OK", "Cancel"))
                {
                    UnityTerrainTreeConverter.RemoveAllPrototypesFromTerrains();
                }
            }

            rect.y += CustomEditorGUI.SingleLineHeight;

            if (CustomEditorGUI.ClickButton(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                    "Unspawn Unity Terrain Tree", ButtonStyle.Remove))
            {
                if (EditorUtility.DisplayDialog("WARNING!",
                        "Are you sure you want to remove all spawned Unity Terrain Tree from the scene?",
                        "OK", "Cancel"))
                {
                    UnityTerrainTreeConverter.UnspawnAllTerrainTree();
                }
            }

            rect.y += CustomEditorGUI.SingleLineHeight;

            if (CustomEditorGUI.ClickButton(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                    "Convert All Unity Terrain Tree To Terrain Object Renderer"))
            {
                UnityTerrainTreeConverter.ConvertAllUnityTerrainTreeToTerrainObjectRenderer();
            }

            rect.y += CustomEditorGUI.SingleLineHeight;

            EditorGUI.HelpBox(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                "Only added prefabs in Renderer are converted when using this button.", MessageType.Info);

            rect.y += CustomEditorGUI.SingleLineHeight;

            if (CustomEditorGUI.ClickButton(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                    "Convert Unity Terrain Tree To Terrain Object Renderer"))
            {
                UnityTerrainTreeConverter.ConvertUnityTerrainTreeToTerrainObjectRenderer();
            }

            rect.y += CustomEditorGUI.SingleLineHeight;

            if (CustomEditorGUI.ClickButton(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                    "Convert Terrain Object Renderer To Unity Terrain Tree"))
            {
                UnityTerrainTreeConverter.ConvertTerrainObjectRendererToUnityTerrainTree();
            }
        }

        public override float GetElementHeight(int index)
        {
            float height = 0;

            height += CustomEditorGUI.SingleLineHeight;
            height += CustomEditorGUI.SingleLineHeight;
            height += CustomEditorGUI.SingleLineHeight;
            height += CustomEditorGUI.SingleLineHeight;
            height += CustomEditorGUI.SingleLineHeight;
            height += CustomEditorGUI.SingleLineHeight;

            return height;
        }
    }
}
#endif
