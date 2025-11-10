#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.ComponentStack.Editor.Core;
using VladislavTsurikov.IMGUIUtility.Editor;
using VladislavTsurikov.IMGUIUtility.Editor.ElementStack;
using VladislavTsurikov.IMGUIUtility.Editor.ElementStack.ReorderableList;
using VladislavTsurikov.MegaWorld.Editor.PrecisePlaceTool.MouseActions;

namespace VladislavTsurikov.MegaWorld.Editor.PrecisePlaceTool.GUI
{
    [DontDrawFoldout]
    [ElementEditor(typeof(PrecisePlaceToolSettings))]
    public class PrecisePlaceToolStackElementEditor : IMGUIElementEditor
    {
        private bool _mouseActionsFoldout = true;
        private IMGUIComponentStackEditor<MouseAction, IMGUIElementEditor> _mouseActionStackEditor;

        private bool _mouseDragSettingsFoldout = true;

        private bool _moveFoldout = true;
        private bool _prototypeToggleSettingsFoldout = true;
        private PrecisePlaceToolSettings _settings;

        public override void OnEnable()
        {
            _settings = (PrecisePlaceToolSettings)Target;
            _mouseActionStackEditor =
                new IMGUIComponentStackEditor<MouseAction, IMGUIElementEditor>(_settings.MouseActionStack);
        }

        public override void OnGUI()
        {
            _settings.RememberPastTransform = CustomEditorGUILayout.Toggle(new GUIContent("Remember Past Transform"),
                _settings.RememberPastTransform);

            _mouseDragSettingsFoldout = CustomEditorGUILayout.Foldout(_mouseDragSettingsFoldout, "Mouse Drag Settings");

            if (_mouseDragSettingsFoldout)
            {
                EditorGUI.indentLevel++;

                _settings.Spacing =
                    Mathf.Max(CustomEditorGUILayout.FloatField(new GUIContent("Spacing"), _settings.Spacing), 0.5f);
                _settings.RandomSelectPrototype =
                    CustomEditorGUILayout.Toggle(new GUIContent("Random Select Prototype"),
                        _settings.RandomSelectPrototype);

                EditorGUI.indentLevel--;
            }

            _prototypeToggleSettingsFoldout =
                CustomEditorGUILayout.Foldout(_prototypeToggleSettingsFoldout, "Prototype Toggle Settings");

            if (_prototypeToggleSettingsFoldout)
            {
                EditorGUI.indentLevel++;

                _settings.UseTransformComponents =
                    CustomEditorGUILayout.Toggle(new GUIContent("Use Transform Components"),
                        _settings.UseTransformComponents);
                _settings.OverlapCheck =
                    CustomEditorGUILayout.Toggle(new GUIContent("Overlap Check"), _settings.OverlapCheck);

                if (_settings.OverlapCheck)
                {
                    EditorGUI.indentLevel++;
                    _settings.VisualizeOverlapCheckSettings = CustomEditorGUILayout.Toggle(
                        new GUIContent("Visualize Overlap Check Settings"), _settings.VisualizeOverlapCheckSettings);
                    EditorGUI.indentLevel--;
                }

                EditorGUI.indentLevel--;
            }

            _moveFoldout = CustomEditorGUILayout.Foldout(_moveFoldout, "Move Action");

            if (_moveFoldout)
            {
                EditorGUI.indentLevel++;

                //settings.EnableSnapMove = CustomEditorGUI.Toggle(new GUIContent("Enable Snap Move"), settings.EnableSnapMove);
                //if(settings.EnableSnapMove)
                //{
                //	EditorGUI.indentLevel++;
                //	settings.SnapMove = CustomEditorGUI.Vector3Field(new GUIContent("Snap Move"), settings.SnapMove);
                //	EditorGUI.indentLevel--;
                //}

                _settings.Align = CustomEditorGUILayout.Toggle(new GUIContent("Align"), _settings.Align);

                if (_settings.Align)
                {
                    EditorGUI.indentLevel++;

                    _settings.WeightToNormal = CustomEditorGUILayout.Slider(new GUIContent("Weight To Normal"),
                        _settings.WeightToNormal, 0, 1);

                    EditorGUI.indentLevel--;
                }

                _settings.AlongStroke =
                    CustomEditorGUILayout.Toggle(new GUIContent("Along Stroke"), _settings.AlongStroke);

                EditorGUI.indentLevel--;
            }

            _mouseActionsFoldout = CustomEditorGUILayout.Foldout(_mouseActionsFoldout, "Mouse Actions");

            if (_mouseActionsFoldout)
            {
                EditorGUI.indentLevel++;

                _mouseActionStackEditor.OnGUI();

                EditorGUI.indentLevel--;
            }
        }
    }
}
#endif
