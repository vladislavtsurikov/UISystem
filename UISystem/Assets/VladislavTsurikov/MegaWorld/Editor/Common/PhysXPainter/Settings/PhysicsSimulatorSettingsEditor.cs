#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.IMGUIUtility.Editor;
using VladislavTsurikov.PhysicsSimulator.Runtime;

namespace VladislavTsurikov.MegaWorld.Editor.Common.PhysXPainter.Settings
{
    public static class PhysicsSimulatorSettingsEditor
    {
        private static bool PhysicsSimulatorSettingsFoldout = true;

        public static void OnGUI<T>(PhysicsSimulatorSettings settings, bool accelerationPhysics = true)
            where T : DisablePhysicsMode
        {
            PhysicsSimulatorSettingsFoldout =
                CustomEditorGUILayout.Foldout(PhysicsSimulatorSettingsFoldout, "Physics Simulator Settings");

            if (!PhysicsSimulatorSettingsFoldout)
            {
                return;
            }

            EditorGUI.indentLevel++;

            EditorGUI.BeginChangeCheck();

            settings.SimulatePhysics = CustomEditorGUILayout.Toggle(
                new GUIContent("Simulate Physics",
                    "You can freeze physics, and then turn on physics again and objects will fall."),
                settings.SimulatePhysics);

            if (typeof(T) == typeof(GlobalTimeDisablePhysicsMode))
            {
                settings.GlobalDisablePhysicsTime = CustomEditorGUILayout.FloatField(
                    new GUIContent("Global Disable Physics Time", "Time for all objects when physics turns off."),
                    settings.GlobalDisablePhysicsTime);
            }
            else
            {
                settings.DisablePhysicsTime = CustomEditorGUILayout.FloatField(
                    new GUIContent("Disable Physics Time", "The time when an object's physics turns off."),
                    settings.DisablePhysicsTime);
            }

            if (accelerationPhysics)
            {
                settings.SpeedUpPhysics = CustomEditorGUILayout.IntSlider(
                    new GUIContent("Speed Up Physics", "Speeds up physics several times as much as was installed."),
                    settings.SpeedUpPhysics, 1, 100);
            }

            PositionOffsetSettingsEditor.OnGUI(settings.AutoPositionDownSettings);

            if (EditorGUI.EndChangeCheck())
            {
                settings.Save();
            }

            EditorGUI.indentLevel--;
        }
    }
}
#endif
