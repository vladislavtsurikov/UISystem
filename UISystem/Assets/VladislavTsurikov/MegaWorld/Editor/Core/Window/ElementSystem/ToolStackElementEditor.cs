#if UNITY_EDITOR
using VladislavTsurikov.ComponentStack.Editor.Core;
using VladislavTsurikov.ComponentStack.Runtime.Core;
using VladislavTsurikov.IMGUIUtility.Editor.ElementStack;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.ElementsSystem;

namespace VladislavTsurikov.MegaWorld.Editor.Core.Window.ElementSystem
{
    [ElementEditor(typeof(ToolComponentStack))]
    public class ToolStackElementEditor : IMGUIElementEditor
    {
        private ToolComponentStack _toolComponentStack;

        public IMGUIComponentStackEditor<Component, IMGUIElementEditor> GeneralComponentStackEditor;

        public override void OnEnable()
        {
            _toolComponentStack = (ToolComponentStack)Target;
            GeneralComponentStackEditor =
                new IMGUIComponentStackEditor<Component, IMGUIElementEditor>(_toolComponentStack.ComponentStack);
        }
    }
}
#endif
