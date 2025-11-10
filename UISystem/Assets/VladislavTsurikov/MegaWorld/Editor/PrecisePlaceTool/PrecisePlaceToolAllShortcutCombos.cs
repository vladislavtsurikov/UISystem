using UnityEngine;
using VladislavTsurikov.EditorShortcutCombo.Editor;
using VladislavTsurikov.Utility.Runtime;

namespace VladislavTsurikov.MegaWorld.Editor.PrecisePlaceTool
{
    public class PrecisePlaceToolAllShortcutCombos : DataTypeSingleton<PrecisePlaceToolAllShortcutCombos>
    {
        public PrecisePlaceToolAllShortcutCombos() => CreateCombos();

        public ShortcutCombo Restore { get; private set; }

        private void CreateCombos()
        {
            Restore = new ShortcutCombo();
            Restore.AddKey(KeyCode.Tab);
        }
    }
}
