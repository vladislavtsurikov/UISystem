#if UNITY_EDITOR
using UnityEngine;
using VladislavTsurikov.ComponentStack.Editor.Core;
using VladislavTsurikov.IMGUIUtility.Editor.ElementStack;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.TransformElementSystem;

namespace VladislavTsurikov.MegaWorld.Editor.Common.Settings.TransformElementSystem
{
    [ElementEditor(typeof(SimpleTransformComponentSettings))]
    public class SimpleTransformComponentSettingsEditor : IMGUIElementEditor
    {
        private SimpleTransformComponentSettings _simpleTransformComponentSettings;
        private TransformStackEditor _transformEditor;

        public override void OnEnable()
        {
            _simpleTransformComponentSettings = (SimpleTransformComponentSettings)Target;
            _transformEditor = new TransformStackEditor(new GUIContent("Transform Component Stack"),
                _simpleTransformComponentSettings.TransformComponentStack, true);
        }

        public override void OnGUI() => _transformEditor.OnGUI();
    }
}
#endif
