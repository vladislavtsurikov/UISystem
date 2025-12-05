#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.IMGUIUtility.Editor;
using VladislavTsurikov.PhysicsSimulator.Runtime;

namespace VladislavTsurikov.MegaWorld.Editor.Common.PhysXPainter.Settings
{
    public static class PositionOffsetSettingsEditor
    {
        private static bool _positionOffsetFoldout = true;

        public static void OnGUI(AutoPositionDownSettings settings)
        {
            _positionOffsetFoldout = CustomEditorGUILayout.Foldout(_positionOffsetFoldout, "Position Offset Settings");

            if (_positionOffsetFoldout)
            {
                EditorGUI.indentLevel++;

                settings.EnableAutoPositionDown = CustomEditorGUILayout.Toggle(
                    new GUIContent("Enable Auto Position Down",
                        "Automatically moves object down when physics turns off."), settings.EnableAutoPositionDown);

                if (settings.EnableAutoPositionDown)
                {
                    EditorGUI.indentLevel++;
                    settings.PositionDown = CustomEditorGUILayout.Slider(
                        new GUIContent("Position Down (%)",
                            "Moves the object down when physics is turned off. 100% - moves the object down by the size of the object."),
                        settings.PositionDown, 0, 100);
                    EditorGUI.indentLevel--;
                }
            }
        }
    }
}
#endif
