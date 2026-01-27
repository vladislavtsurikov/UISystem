using System;
using System.Collections.Generic;
using OdinSerializer;
using VladislavTsurikov.EntityDataAction.Runtime;
using VladislavTsurikov.EntityDataAction.Runtime.Core;
using VladislavTsurikov.ReflectionUtility;

namespace VladislavTsurikov.EntityDataAction.Shared.Runtime.Style
{
    [Name("UI/Common/Style/StyleStateData")]
    public sealed class StyleStateData : ComponentData
    {
        private const string DefaultStyle = "Normal";

        public event Action<string, bool> OnStyleChanged;

        [OdinSerialize]
        private HashSet<string> _activeStyles = new HashSet<string> { DefaultStyle };

        public bool HasStyle(string styleName)
        {
            if (string.IsNullOrWhiteSpace(styleName))
            {
                return false;
            }

            return _activeStyles.Contains(styleName);
        }

        public void AddStyle(string styleName)
        {
            if (string.IsNullOrWhiteSpace(styleName))
            {
                return;
            }

            if (styleName != DefaultStyle && _activeStyles.Contains(DefaultStyle))
            {
                _activeStyles.Remove(DefaultStyle);
                OnStyleChanged?.Invoke(DefaultStyle, false);
            }

            bool changed = _activeStyles.Add(styleName);
            if (!changed)
            {
                return;
            }

            MarkDirty();
            OnStyleChanged?.Invoke(styleName, true);
        }

        public void RemoveStyle(string styleName)
        {
            if (string.IsNullOrWhiteSpace(styleName))
            {
                return;
            }

            bool changed = _activeStyles.Remove(styleName);
            if (!changed)
            {
                return;
            }

            MarkDirty();
            OnStyleChanged?.Invoke(styleName, false);

            if (_activeStyles.Count == 0)
            {
                _activeStyles.Add(DefaultStyle);
                MarkDirty();
                OnStyleChanged?.Invoke(DefaultStyle, true);
            }
        }

        public void ClearStyles()
        {
            if (_activeStyles.Count == 0)
            {
                return;
            }

            List<string> stylesToRemove = new List<string>(_activeStyles);
            _activeStyles.Clear();
            MarkDirty();

            foreach (string styleName in stylesToRemove)
            {
                OnStyleChanged?.Invoke(styleName, false);
            }
        }

        public void Reset()
        {
            _activeStyles.Add(DefaultStyle);
            MarkDirty();
            OnStyleChanged?.Invoke(DefaultStyle, true);
        }
    }
}
