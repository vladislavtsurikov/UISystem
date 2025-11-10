#if UNITY_EDITOR
using UnityEngine;
using VladislavTsurikov.ComponentStack.Editor.Core;
using VladislavTsurikov.IMGUIUtility.Editor;
using VladislavTsurikov.IMGUIUtility.Editor.ElementStack;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings;

namespace VladislavTsurikov.MegaWorld.Editor.Common.Settings
{
    [ElementEditor(typeof(LayerSettings))]
    public class LayerSettingsEditor : IMGUIElementEditor
    {
        private LayerSettings _layerSettings => (LayerSettings)Target;

        public override void OnGUI() =>
            _layerSettings.PaintLayers = CustomEditorGUILayout.LayerField(new GUIContent("Paint Layers",
                "Allows you to set the layers on which to spawn."), _layerSettings.PaintLayers);
    }
}
#endif
