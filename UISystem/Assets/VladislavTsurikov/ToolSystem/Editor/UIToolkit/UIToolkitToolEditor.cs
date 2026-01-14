#if UNITY_EDITOR
using UnityEngine.UIElements;
using VladislavTsurikov.ComponentStack.Editor.Core;

namespace VladislavTsurikov.ToolSystem.Editor.UIToolkit
{
    /// <summary>
    /// Base class for UIToolkit-based tool editors.
    /// Tool editors create visual UI for tools when they are selected in ToolStack.
    /// </summary>
    public abstract class UIToolkitToolEditor : ElementEditor
    {
        private VisualElement _contentContainer;

        /// <summary>
        /// Creates the visual element for this tool editor.
        /// This method is called when the tool is selected and needs to display its UI.
        /// </summary>
        public virtual VisualElement CreateGUI()
        {
            if (_contentContainer == null)
            {
                _contentContainer = CreateVisualElement();
            }
            return _contentContainer;
        }

        /// <summary>
        /// Override this method to create the visual UI for your tool.
        /// Return a VisualElement that will be displayed when the tool is selected.
        /// </summary>
        protected abstract VisualElement CreateVisualElement();

        /// <summary>
        /// Called to update the visual element.
        /// Override this if you need to refresh the UI when the tool state changes.
        /// </summary>
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
