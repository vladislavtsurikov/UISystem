#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEngine;
using VladislavTsurikov.ComponentStack.Editor.Core;
using VladislavTsurikov.IMGUIUtility.Editor.ElementStack;
using VladislavTsurikov.MegaWorld.Runtime.Common.PhysXPainter.Settings;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.TransformElementSystem;

namespace VladislavTsurikov.MegaWorld.Editor.Common.Settings.TransformElementSystem
{
    [ElementEditor(typeof(PhysicsTransformComponentSettings))]
    public class PhysicsTransformComponentSettingsEditor : IMGUIElementEditor
    {
        private PhysicsTransformComponentSettings _physicsTransformComponentSettings;

        private TransformStackEditor _transformEditor;

        public override void OnEnable()
        {
            _physicsTransformComponentSettings = (PhysicsTransformComponentSettings)Target;
            var types = new List<Type> { typeof(Align) };
            _transformEditor = new TransformStackEditor(new GUIContent("Transform Components Settings"),
                _physicsTransformComponentSettings.TransformComponentStack, types, true);
        }

        public override void OnGUI() => _transformEditor.OnGUI();
    }
}
#endif
