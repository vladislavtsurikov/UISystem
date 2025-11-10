#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.ComponentStack.Editor.Core;
using VladislavTsurikov.IMGUIUtility.Editor;
using VladislavTsurikov.IMGUIUtility.Editor.ElementStack;
using VladislavTsurikov.IMGUIUtility.Editor.ElementStack.ReorderableList;
using VladislavTsurikov.MegaWorld.Editor.EditTool.ActionSystem;

namespace VladislavTsurikov.MegaWorld.Editor.EditTool
{
    [DontDrawFoldout]
    [ElementEditor(typeof(EditToolSettings))]
    public class EditToolSettingsEditor : IMGUIElementEditor
    {
        private EditToolSettings _editToolSettings;
        private bool _hotkeysFoldout;

        private static GUIStyle TitleStyle
        {
            get
            {
                var guiStyle = new GUIStyle { richText = true };

                return guiStyle;
            }
        }

        public ActionStackEditor ActionStackEditor { get; private set; }

        public override void OnEnable()
        {
            _editToolSettings = (EditToolSettings)Target;
            ActionStackEditor = new ActionStackEditor(_editToolSettings.ActionStack);
        }

        public override void OnGUI()
        {
            ActionStackEditor.DrawSettings();

            HotKeys();
        }

        private void HotKeys()
        {
            _hotkeysFoldout = CustomEditorGUILayout.Foldout(_hotkeysFoldout, "Hotkeys");

            if (_hotkeysFoldout)
            {
                EditorGUI.indentLevel++;

                CustomEditorGUILayout.Label(
                    "<size=14><color=#" + UnityEngine.ColorUtility.ToHtmlStringRGB(EditorColors.Instance.LabelColor) +
                    ">" + "<b><i>Q</i></b> - Move Up/Down.</color></size>", TitleStyle);
                CustomEditorGUILayout.Label(
                    "<size=14><color=#" + UnityEngine.ColorUtility.ToHtmlStringRGB(EditorColors.Instance.LabelColor) +
                    ">" + "<b><i>W</i></b> - Raycast.</color></size>", TitleStyle);

                CustomEditorGUILayout.Label(
                    "<size=14><color=#" + UnityEngine.ColorUtility.ToHtmlStringRGB(EditorColors.Instance.LabelColor) +
                    ">" + "<b><i>E</i></b> - Rotation.</color></size>", TitleStyle);
                EditorGUI.indentLevel++;
                CustomEditorGUILayout.Label(
                    "<size=14><color=#" + UnityEngine.ColorUtility.ToHtmlStringRGB(EditorColors.Instance.LabelColor) +
                    ">" + "<b><i>Space</i></b> - Transform Space.</color></size>", TitleStyle);
                EditorGUI.indentLevel--;

                CustomEditorGUILayout.Label(
                    "<size=14><color=#" + UnityEngine.ColorUtility.ToHtmlStringRGB(EditorColors.Instance.LabelColor) +
                    ">" + "<b><i>R</i></b> - Scale.</color></size>", TitleStyle);
                CustomEditorGUILayout.Label(
                    "<size=14><color=#" + UnityEngine.ColorUtility.ToHtmlStringRGB(EditorColors.Instance.LabelColor) +
                    ">" + "<b><i>T</i></b> - Remove.</color></size>", TitleStyle);

                EditorGUI.indentLevel--;
            }
        }
    }
}
#endif
