#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.IMGUIUtility.Editor;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group;
using VladislavTsurikov.MegaWorld.Runtime.TerrainSpawner;

namespace VladislavTsurikov.MegaWorld.Editor.TerrainSpawner
{
    [CustomEditor(typeof(TerrainMask))]
    public class TerrainMaskEditor : UnityEditor.Editor
    {
        private TerrainMask _terrainMask;

        private void OnEnable() => _terrainMask = (TerrainMask)target;

        public override void OnInspectorGUI()
        {
            _terrainMask.Group = (Group)CustomEditorGUILayout.ObjectField(new GUIContent("Group"),
                _terrainMask.Group == null,
                _terrainMask.Group, typeof(Group));

            _terrainMask.Mask = (Texture2D)CustomEditorGUILayout.ObjectField(new GUIContent("Mask"),
                _terrainMask.Mask == null,
                _terrainMask.Mask, typeof(Texture2D));
        }
    }
}
#endif
