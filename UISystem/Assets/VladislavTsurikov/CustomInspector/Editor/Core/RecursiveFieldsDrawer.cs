using System.Collections.Generic;

namespace VladislavTsurikov.CustomInspector.Editor.Core
{
    public abstract class RecursiveFieldsDrawer
    {
        private readonly Dictionary<object, bool> _foldoutStates = new();

        protected bool GetFoldoutState(object value)
        {
            if (value == null)
            {
                return false;
            }

            if (!_foldoutStates.TryGetValue(value, out var state))
            {
                _foldoutStates[value] = false;
                return false;
            }

            return state;
        }

        protected void SetFoldoutState(object value, bool state)
        {
            if (value == null)
            {
                return;
            }

            _foldoutStates[value] = state;
        }
    }
}
