#if UNITY_EDITOR
using VladislavTsurikov.Nody.Editor.Core;
using VladislavTsurikov.Core.Editor;
using VladislavTsurikov.IMGUIUtility.Editor.ElementStack;
using VladislavTsurikov.RendererStack.Runtime.Core.GlobalSettings;

namespace VladislavTsurikov.RendererStack.Editor.Core.GlobalSettings
{
    [ElementEditor(typeof(RendererGlobalComponentStack))]
    public class RendererGlobalComponentStackEditor : IMGUIElementEditor
    {
        private IMGUIComponentStackEditor<GlobalComponent, IMGUIElementEditor> _componentStackEditor;

        public override void OnEnable() =>
            _componentStackEditor =
                new IMGUIComponentStackEditor<GlobalComponent, IMGUIElementEditor>(
                    ((RendererGlobalComponentStack)Target).ComponentStack);

        public override void OnGUI() => _componentStackEditor.OnGUI();
    }
}
#endif
