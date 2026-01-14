#if UNITY_EDITOR
using VladislavTsurikov.Nody.Runtime.AdvancedNodeStack;

namespace VladislavTsurikov.MegaWorld.Editor.EditTool.ActionSystem
{
    [CreateNodes(new[]
    {
        typeof(MoveAlongDirection), typeof(Raycast), typeof(Rotate), typeof(Scale), typeof(Remove)
    })]
    public class ActionStack : NodeStackOnlyDifferentTypes<Action>
    {
        public void CheckShortcutCombos()
        {
            foreach (Action settings in _elementList)
            {
                if (settings.CheckShortcutCombo())
                {
                    Select(settings);
                    return;
                }
            }
        }
    }
}
#endif
