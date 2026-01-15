#if UNITY_EDITOR
using UnityEngine;
using VladislavTsurikov.IMGUIUtility.Editor.ElementStack.ReorderableList;
using VladislavTsurikov.Nody.Runtime.AdvancedNodeStack;
using VladislavTsurikov.SceneManagerTool.Runtime.SceneTypeSystem;

namespace VladislavTsurikov.SceneManagerTool.Editor.SceneTypeSystem
{
    public class SceneTypeComponentStackEditor : ReorderableListStackEditor<SceneType, SceneTypeEditor>
    {
        public SceneTypeComponentStackEditor(GUIContent reorderableListName,
            NodeStackSupportSameType<SceneType> list) : base(reorderableListName, list, true)
        {
            CopySettings = true;
            ShowActiveToggle = false;
        }
    }
}
#endif
