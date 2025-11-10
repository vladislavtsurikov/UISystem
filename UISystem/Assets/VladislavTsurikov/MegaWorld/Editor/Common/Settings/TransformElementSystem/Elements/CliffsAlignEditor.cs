#if UNITY_EDITOR
using UnityEditor;
using VladislavTsurikov.ComponentStack.Editor.Core;
using VladislavTsurikov.IMGUIUtility.Editor.ElementStack.ReorderableList;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.TransformElementSystem;

namespace VladislavTsurikov.MegaWorld.Editor.Common.Settings.TransformElementSystem
{
    [ElementEditor(typeof(CliffsAlign))]
    public class CliffsAlignEditor : ReorderableListComponentEditor
    {
        public override float GetElementHeight(int index)
        {
            var height = EditorGUIUtility.singleLineHeight;

            return height;
        }
    }
}
#endif
