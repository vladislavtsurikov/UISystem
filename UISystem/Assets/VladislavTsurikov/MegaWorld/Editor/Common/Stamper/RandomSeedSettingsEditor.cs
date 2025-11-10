#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.ComponentStack.Editor.Core;
using VladislavTsurikov.IMGUIUtility.Editor;
using VladislavTsurikov.IMGUIUtility.Editor.ElementStack;
using VladislavTsurikov.IMGUIUtility.Editor.ElementStack.ReorderableList;
using VladislavTsurikov.MegaWorld.Runtime.Common.Stamper;

namespace VladislavTsurikov.MegaWorld.Editor.Common.Stamper
{
    [DontDrawFoldout]
    [ElementEditor(typeof(RandomSeedSettings))]
    public class RandomSeedSettingsEditor : IMGUIElementEditor
    {
        private RandomSeedSettings _settings => (RandomSeedSettings)Target;

        public override void OnGUI()
        {
            _settings.GenerateRandomSeed =
                CustomEditorGUILayout.Toggle(new GUIContent("Generate Random Seed"), _settings.GenerateRandomSeed);
            if (_settings.GenerateRandomSeed)
            {
                EditorGUI.indentLevel++;

                _settings.RandomSeed =
                    CustomEditorGUILayout.IntField(new GUIContent("Random Seed"), _settings.RandomSeed);

                EditorGUI.indentLevel--;
            }
        }
    }
}
#endif
