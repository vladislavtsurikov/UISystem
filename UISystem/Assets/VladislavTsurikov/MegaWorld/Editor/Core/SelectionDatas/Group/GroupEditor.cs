#if UNITY_EDITOR
using UnityEditor;
using VladislavTsurikov.IMGUIUtility.Editor;
using VladislavTsurikov.MegaWorld.Editor.Core.SelectionDatas.Group.Prototypes;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas;

namespace VladislavTsurikov.MegaWorld.Editor.Core.SelectionDatas.Group
{
    [CustomEditor(typeof(Runtime.Core.SelectionDatas.Group.Group))]
    public class GroupEditor : UnityEditor.Editor
    {
        public SelectionData SelectionData = new();
        private Runtime.Core.SelectionDatas.Group.Group _group;

        public IconPrototypesDrawer PrototypesDrawer;

        private void OnEnable() => _group = (Runtime.Core.SelectionDatas.Group.Group)target;

        public override void OnInspectorGUI()
        {
            CustomEditorGUILayout.IsInspector = true;
            OnGUI();
        }

        public void OnGUI()
        {
            var initionGroupSelected = _group.Selected;

            _group.Selected = true;

            SelectionData = new SelectionData();
            SelectionData.Setup();
            SelectionData.GroupList.Add(_group);
            SelectionData.SelectedData.SetSelectedData();

            if (PrototypesDrawer == null)
            {
                PrototypesDrawer = new IconPrototypesDrawer(SelectionData, null);
            }

            PrototypesDrawer.OnGUI();

            _group.Selected = initionGroupSelected;
        }
    }
}
#endif
