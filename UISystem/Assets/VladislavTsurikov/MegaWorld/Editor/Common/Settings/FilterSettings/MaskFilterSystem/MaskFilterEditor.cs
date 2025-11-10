#if UNITY_EDITOR
using VladislavTsurikov.IMGUIUtility.Editor.ElementStack.ReorderableList;

namespace VladislavTsurikov.MegaWorld.Editor.Common.Settings.FilterSettings.MaskFilterSystem
{
    public abstract class MaskFilterEditor : ReorderableListComponentEditor
    {
        public virtual string GetAdditionalName() => "";
    }
}
#endif
