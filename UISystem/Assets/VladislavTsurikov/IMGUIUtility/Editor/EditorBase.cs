#if UNITY_EDITOR
namespace VladislavTsurikov.IMGUIUtility.Editor
{
    public class EditorBase : UnityEditor.Editor
    {
        public override bool RequiresConstantRepaint() => true;
    }
}
#endif
