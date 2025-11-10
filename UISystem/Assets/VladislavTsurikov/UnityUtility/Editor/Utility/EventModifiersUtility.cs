using UnityEngine;

namespace VladislavTsurikov.UnityUtility.Editor
{
    public static class EventModifiersUtility
    {
        public static bool IsModifierDown(EventModifiers modifiers)
        {
            Event e = Event.current;

            if ((e.modifiers & EventModifiers.FunctionKey) != 0)
            {
                return false;
            }

            EventModifiers mask = EventModifiers.Alt | EventModifiers.Control | EventModifiers.Shift |
                                  EventModifiers.Command;
            modifiers &= mask;

            if (modifiers == 0 && (e.modifiers & mask & ~modifiers) == 0)
            {
                return true;
            }

            return (e.modifiers & modifiers) != 0 && (e.modifiers & mask & ~modifiers) == 0;
        }
    }
}
