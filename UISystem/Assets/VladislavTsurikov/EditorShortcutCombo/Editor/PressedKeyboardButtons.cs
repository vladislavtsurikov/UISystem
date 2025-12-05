using System.Collections.Generic;
using UnityEngine;

namespace VladislavTsurikov.EditorShortcutCombo.Editor
{
    public class PressedKeyboardButtons
    {
        private readonly List<KeyCode> _keyList = new();

        public int NumberOfKeys => _keyList.Count;

        public void OnKeyboardButtonPressed(KeyCode keyCode) => AddKeyCodeIfNecessary(keyCode);

        public void OnKeyboardButtonReleased(KeyCode keyCode) => DeleteKeyCode(keyCode);

        public bool IsKeyboardButtonPressed(KeyCode keyCode)
        {
            if (DoesKeyCodeEntryExist(keyCode))
            {
                return true;
            }

            return false;
        }

        public void AddKeyCodeIfNecessary(KeyCode keyCode)
        {
            if (!DoesKeyCodeEntryExist(keyCode))
            {
                _keyList.Add(keyCode);
            }
        }

        public void DeleteKeyCode(KeyCode keyCode) => _keyList.Remove(keyCode);

        private bool DoesKeyCodeEntryExist(KeyCode keyCode) => _keyList.Contains(keyCode);
    }
}
