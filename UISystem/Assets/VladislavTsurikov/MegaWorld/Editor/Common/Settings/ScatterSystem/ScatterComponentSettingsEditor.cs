#if UNITY_EDITOR
using UnityEngine;
using VladislavTsurikov.ComponentStack.Editor.Core;
using VladislavTsurikov.IMGUIUtility.Editor.ElementStack;
using VladislavTsurikov.IMGUIUtility.Editor.ElementStack.ReorderableList;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.ScatterSystem;

namespace VladislavTsurikov.MegaWorld.Editor.Common.Settings.ScatterSystem
{
    [ElementEditor(typeof(ScatterComponentSettings))]
    public class ScatterComponentSettingsEditor : IMGUIElementEditor
    {
        private ReorderableListStackEditor<Scatter, ReorderableListComponentEditor> _stackEditor;

        public override void OnEnable()
        {
            var scatterComponentSettings = (ScatterComponentSettings)Target;
            _stackEditor =
                new ReorderableListStackEditor<Scatter, ReorderableListComponentEditor>(
                    new GUIContent("Scatter Operations"), scatterComponentSettings.ScatterStack, true)
                {
                    DisplayHeaderText = false
                };
        }

        public override void OnGUI() => _stackEditor.OnGUI();
    }
}
#endif
