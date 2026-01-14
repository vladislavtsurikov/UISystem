#if UNITY_EDITOR
using UnityEngine;
using VladislavTsurikov.Nody.Runtime.AdvancedNodeStack;
using VladislavTsurikov.IMGUIUtility.Editor.ElementStack.ReorderableList;
using VladislavTsurikov.RendererStack.Runtime.Core.PrototypeRendererSystem.PrototypeSettings;

namespace VladislavTsurikov.RendererStack.Editor.Core.PrototypeRendererSystem.PrototypeSettings
{
    public class
        PrototypeComponentStackEditor : ReorderableListStackEditor<PrototypeComponent, PrototypeComponentEditor>
    {
        private readonly NodeStackOnlyDifferentTypes<PrototypeComponent> _componentStackOnlyDifferentTypes;

        public PrototypeComponentStackEditor(NodeStackOnlyDifferentTypes<PrototypeComponent> stack) :
            base(new GUIContent(""), stack, true) =>
            _componentStackOnlyDifferentTypes = stack;
    }
}
#endif
