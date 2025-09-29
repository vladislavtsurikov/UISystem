using System;
using System.Collections.Generic;

namespace VladislavTsurikov.UISystem.Runtime.UnityUIIntegration
{
    internal class UIBindingRepeatTracker
    {
        private readonly Dictionary<(Type, string), int> _counters = new();

        public int GetAndIncrement(Type type, string bindingId)
        {
            (Type type, string) key = (type, bindingId ?? "");
            if (!_counters.TryGetValue(key, out var index))
            {
                _counters[key] = 1;
                return 0;
            }

            _counters[key] = index + 1;
            return index;
        }

        public void Reset() => _counters.Clear();
    }
}
