#if UNITY_EDITOR
using UnityEngine;
using VladislavTsurikov.ComponentStack.Editor.Core;
using VladislavTsurikov.IMGUIUtility.Editor.ElementStack;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.TransformElementSystem;

namespace VladislavTsurikov.MegaWorld.Editor.Common.Settings.TransformElementSystem
{
    [ElementEditor(typeof(TransformComponentSettings))]
    public class TransformComponentSettingsEditor : IMGUIElementEditor
    {
        private TransformComponentSettings _transformComponentSettings;
        private TransformStackEditor _transformEditor;

        public override void OnEnable()
        {
            _transformComponentSettings = (TransformComponentSettings)Target;
            _transformEditor = new TransformStackEditor(new GUIContent("Transform Component Stack"),
                _transformComponentSettings.TransformComponentStack);
        }

        public override void OnGUI() => _transformEditor.OnGUI();
    }
}
#endif
