using System;

namespace VladislavTsurikov.CustomInspector.Runtime
{
    /// <summary>
    /// Displays a method as a button in the inspector.
    /// The method must have no parameters or only optional parameters.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class ButtonAttribute : Attribute
    {
        /// <summary>
        /// Create a button attribute
        /// </summary>
        /// <param name="buttonSize">Size of the button</param>
        /// <param name="name">Custom button text (uses method name if null)</param>
        public ButtonAttribute(ButtonSize buttonSize = ButtonSize.Medium, string name = null)
        {
            ButtonSize = buttonSize;
            Name = name;
        }

        /// <summary>
        /// Create a button with custom name
        /// </summary>
        public ButtonAttribute(string name)
        {
            ButtonSize = ButtonSize.Medium;
            Name = name;
        }

        public ButtonSize ButtonSize { get; }
        public string Name { get; }
    }

    public enum ButtonSize
    {
        Small,
        Medium,
        Large
    }
}
