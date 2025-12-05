#if UNITY_EDITOR
using UnityEditorInternal;
using UnityEngine;
using VladislavTsurikov.ComponentStack.Runtime.AdvancedComponentStack;
using VladislavTsurikov.IMGUIUtility.Editor;
using VladislavTsurikov.IMGUIUtility.Editor.ElementStack.ReorderableList;
using VladislavTsurikov.SceneManagerTool.Runtime.SceneCollectionSystem;

namespace VladislavTsurikov.SceneManagerTool.Editor.SceneCollectionSystem
{
    public class SceneCollectionStackEditor : ReorderableListStackEditor<SceneCollection, SceneCollectionEditor>
    {
        public SceneCollectionStackEditor(GUIContent reorderableListName,
            ComponentStackSupportSameType<SceneCollection> list) : base(reorderableListName, list, true)
        {
            RenameSupport = true;
            CopySettings = true;
        }

        protected override void AddCB(ReorderableList list)
        {
            var componentStackSupportSameType = (ComponentStackSupportSameType<SceneCollection>)Stack;
            componentStackSupportSameType.CreateComponent(typeof(SceneCollection));
        }

        protected override void DrawHeaderElement(Rect headerRect, int index, SceneCollectionEditor componentEditor) =>
            componentEditor.Target.SelectSettingsFoldout =
                CustomEditorGUI.HeaderWithMenu(headerRect, componentEditor.Target.Name,
                    componentEditor.Target.SelectSettingsFoldout, () => Menu(Stack.ElementList[index], index));
    }
}
#endif
