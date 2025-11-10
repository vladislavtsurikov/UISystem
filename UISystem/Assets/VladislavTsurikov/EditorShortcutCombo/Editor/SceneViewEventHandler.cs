using UnityEngine;

namespace VladislavTsurikov.EditorShortcutCombo.Editor
{
    public static class SceneViewEventHandler
    {
        public static PressedKeyboardButtons PressedKeyboardButtons { get; } = new();

        public static void HandleSceneViewEvent(Event e)
        {
            switch (e.type)
            {
                case EventType.KeyDown:
                {
                    //When C is pressed, EventType.KeyUp does not fire
                    if (e.keyCode == KeyCode.C)
                    {
                        return;
                    }

                    PressedKeyboardButtons.OnKeyboardButtonPressed(e.keyCode);
                    break;
                }
                case EventType.KeyUp:
                {
                    PressedKeyboardButtons.OnKeyboardButtonReleased(e.keyCode);
                    break;
                }
            }

            PressedKeyboardButtons.DeleteKeyCode(KeyCode.None);
        }
    }
}
