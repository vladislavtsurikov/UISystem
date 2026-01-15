#if UNITY_EDITOR
using UnityEngine.UIElements;
using VladislavTsurikov.Nody.Editor.Core;

namespace VladislavTsurikov.ToolSystem.Editor.UIToolkit
{
    public abstract class UIToolkitToolEditor : ElementEditor
    {
        private VisualElement _contentContainer;

        public virtual VisualElement CreateGUI()
        {
            if (_contentContainer == null)
            {
                _contentContainer = CreateVisualElement();
            }
            return _contentContainer;
        }

        protected abstract VisualElement CreateVisualElement();

        public virtual void UpdateGUI()
        {
            if (_contentContainer != null)
            {
                _contentContainer.Clear();
                var newContent = CreateVisualElement();
                foreach (var child in newContent.Children())
                {
                    _contentContainer.Add(child);
                }
            }
        }
    }
}
#endif
