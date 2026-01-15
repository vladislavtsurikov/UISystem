#if UNITY_EDITOR
using VladislavTsurikov.Nody.Editor.Core;
using VladislavTsurikov.Nody.Runtime.Core;
using VladislavTsurikov.Core.Editor;
using VladislavTsurikov.IMGUIUtility.Editor.ElementStack;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.ElementsSystem;

namespace VladislavTsurikov.MegaWorld.Editor.Core.Window.ElementSystem
{
    [ElementEditor(typeof(ToolComponentStack))]
    public class ToolStackElementEditor : IMGUIElementEditor
    {
        private ToolComponentStack _toolComponentStack;

        public IMGUIComponentStackEditor<Node, IMGUIElementEditor> GeneralComponentStackEditor;

        public override void OnEnable()
        {
            _toolComponentStack = (ToolComponentStack)Target;
            GeneralComponentStackEditor =
                new IMGUIComponentStackEditor<Node, IMGUIElementEditor>(_toolComponentStack.ComponentStack);
        }
    }
}
#endif
