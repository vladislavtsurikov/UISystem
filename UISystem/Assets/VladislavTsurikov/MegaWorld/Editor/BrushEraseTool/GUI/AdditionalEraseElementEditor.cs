#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.Nody.Editor.Core;
using VladislavTsurikov.Core.Editor;
using VladislavTsurikov.IMGUIUtility.Editor;
using VladislavTsurikov.IMGUIUtility.Editor.ElementStack;
using VladislavTsurikov.IMGUIUtility.Editor.ElementStack.ReorderableList;
using VladislavTsurikov.MegaWorld.Editor.BrushEraseTool.PrototypeElements;

namespace VladislavTsurikov.MegaWorld.Editor.BrushEraseTool
{
    [DontDrawFoldout]
    [ElementEditor(typeof(AdditionalEraseSetting))]
    public class AdditionalEraseElementEditor : IMGUIElementEditor
    {
        private readonly GUIContent _success = new("Success of Erase (%)");
        private AdditionalEraseSetting _additionalEraseSetting;

        public override void OnEnable() => _additionalEraseSetting = (AdditionalEraseSetting)Target;

        public override void OnGUI() =>
            _additionalEraseSetting.Success =
                EditorGUILayout.Slider(_success, _additionalEraseSetting.Success, 0f, 100f);
    }
}
#endif
