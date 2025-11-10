#if UNITY_EDITOR
using VladislavTsurikov.ComponentStack.Editor.Core;
using VladislavTsurikov.IMGUIUtility.Editor.ElementStack;

namespace VladislavTsurikov.RendererStack.Editor.TerrainObjectRenderer.GlobalSettings.ExtensionSystem
{
    [ElementEditor(typeof(Runtime.TerrainObjectRenderer.GlobalSettings.ExtensionSystem.ExtensionSystem))]
    public class ExtensionSystemEditor : IMGUIElementEditor
    {
        private Runtime.TerrainObjectRenderer.GlobalSettings.ExtensionSystem.ExtensionSystem _extensionSystem;
        private ExtensionStackEditor _settingsStackEditor;

        public override void OnEnable()
        {
            _extensionSystem = (Runtime.TerrainObjectRenderer.GlobalSettings.ExtensionSystem.ExtensionSystem)Target;
            _settingsStackEditor = new ExtensionStackEditor(_extensionSystem.ExtensionStack);
        }

        public override void OnGUI() => _settingsStackEditor.OnGUI();
    }
}
#endif
