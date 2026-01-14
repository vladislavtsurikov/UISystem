# UIToolkit ReorderableList for ComponentStack

A comprehensive UIToolkit implementation of ReorderableList specifically designed for ComponentStack system, recreating the complex UI and functionality of the IMGUI version.

## Overview

This implementation provides a modern UIToolkit-based interface for managing component stacks, matching the appearance and behavior of the original IMGUI `ReorderableListStackEditor`.

## Features

### Visual Design
- **Header with Component Name**: Bold header displaying component name
- **Active Toggle**: Optional toggle for enabling/disabling components
- **Foldout**: Expandable content area for component properties
- **Drag Handle**: Visual handle for reordering components
- **Context Menu**: Right-click menu with component operations
- **Selection Highlighting**: Visual feedback for selected components
- **Rename Support**: In-place renaming with Enter/Escape keyboard support

### Functionality
- **Drag and Drop Reordering**: Full support for reordering components via drag handle
- **Add Components**: Plus button with type selection menu
- **Remove Components**: Minus button and context menu option
- **Duplicate Components**: Deep copy of component with all settings
- **Reset Components**: Reset to default values
- **Copy/Paste Settings**: Copy component settings and paste to another
- **Rename Components**: In-place renaming with validation
- **Active State Management**: Toggle component active state
- **Foldout State Persistence**: Remembers expanded/collapsed state

## Components

### UIToolkitReorderableListStackEditor
Main editor class managing the entire component stack.

**Type Parameters:**
- `T` - Component type (must inherit from `Component`)
- `N` - Editor type (must inherit from `UIToolkitReorderableListComponentEditor`)

**Properties:**
- `DisplayHeaderText` - Show/hide list header
- `DisplayPlusButton` - Show/hide add button
- `DuplicateSupport` - Enable/disable component duplication
- `RenameSupport` - Enable/disable renaming
- `ShowActiveToggle` - Show/hide active toggles
- `RemoveSupport` - Enable/disable component removal
- `ReorderSupport` - Enable/disable drag and drop
- `CopySettings` - Enable/disable copy/paste functionality

### UIToolkitReorderableListComponentEditor
Base editor class for individual component editing.

**Key Methods:**
- `CreateGUI()` - Creates the visual element for component fields
- `UpdateGUI()` - Updates the visual representation

### ComponentListElement
Visual element representing a single component in the list.

**Features:**
- Header container with foldout
- Drag handle for reordering
- Active toggle
- Context menu button
- Content area for fields
- Rename UI with confirmation/cancel buttons
- Selection state visualization

## Usage Example

```csharp
using VladislavTsurikov.IMGUIUtility.Editor.ElementStack.UIToolkitReorderableList;
using VladislavTsurikov.ComponentStack.Runtime.AdvancedComponentStack;
using VladislavTsurikov.ComponentStack.Runtime.Core;

public class MyEditorWindow : EditorWindow
{
    private ComponentStackSupportSameType<MyComponent> _componentStack;
    private UIToolkitReorderableListStackEditor<MyComponent, UIToolkitReorderableListComponentEditor> _stackEditor;

    public void CreateGUI()
    {
        // Initialize component stack
        _componentStack = new ComponentStackSupportSameType<MyComponent>();
        _componentStack.Setup(true);

        // Create the stack editor
        _stackEditor = new UIToolkitReorderableListStackEditor<MyComponent, UIToolkitReorderableListComponentEditor>(
            new GUIContent("My Components"),
            _componentStack,
            true)
        {
            DisplayHeaderText = true,
            DisplayPlusButton = true,
            DuplicateSupport = true,
            RenameSupport = true,
            ShowActiveToggle = true,
            RemoveSupport = true,
            ReorderSupport = true,
            CopySettings = true
        };

        // Add to UI
        var stackElement = _stackEditor.GetVisualElement();
        rootVisualElement.Add(stackElement);
    }
}
```

## Visual Appearance

The implementation recreates the IMGUI appearance:

```
┌──────────────────────────────────────────┐
│ ☰ > Component Name           ☑ Active ⋮ │  ← Header
├──────────────────────────────────────────┤
│     Field 1: [value]                     │  ← Content (when expanded)
│     Field 2: [value]                     │
│     Field 3: [value]                     │
└──────────────────────────────────────────┘
```

**Color Scheme:**
- Background: Dark gray (#383838)
- Selected: Lighter gray (#424242) with blue left border
- Header: Bold text
- Drag Handle: Gray with grip lines
- Borders: Dark (#202020)

## Context Menu Options

- **Reset** - Reset component to default values
- **Remove** - Delete component (if deletable)
- **Duplicate** - Create a copy of the component
- **Rename** - Rename the component
- **Copy Settings** - Copy component configuration
- **Paste Settings** - Paste previously copied settings

## Rename Functionality

When rename is triggered:
1. Header is hidden
2. Rename UI appears with text field
3. User can type new name
4. Press Enter or click ✓ to accept
5. Press Escape or click ✗ to cancel
6. Returns to normal view

## Integration with ComponentStack

This implementation works seamlessly with:
- `ComponentStack<T>` - Base stack class
- `ComponentStackSupportSameType<T>` - Allows multiple components of same type
- `ComponentStackOnlyDifferentTypes<T>` - Allows only unique component types
- `AdvancedComponentStack<T>` - Advanced stack features

## Styling

Custom styles are provided in `ComponentListStyles.uss`:
- `.component-list-element` - Main element style
- `.component-list-element__main` - Background container
- `.component-list-element__header` - Header area
- `.component-list-element__drag-handle` - Drag handle styling
- `.component-list-element__content` - Content area
- `.component-list-element--selected` - Selected state

## Comparison with IMGUI Version

| Feature | IMGUI | UIToolkit |
|---------|-------|-----------|
| Drag & Drop | ✓ | ✓ |
| Add/Remove | ✓ | ✓ |
| Context Menu | ✓ | ✓ |
| Rename | ✓ | ✓ |
| Copy/Paste | ✓ | ✓ |
| Active Toggle | ✓ | ✓ |
| Foldout | ✓ | ✓ |
| Visual Match | N/A | ✓ |
| Performance | Good | Better |

## Requirements

- Unity 2021.3 or newer
- VladislavTsurikov.ComponentStack
- VladislavTsurikov.CustomInspector
- VladislavTsurikov.UIToolkitReorderableList
- VladislavTsurikov.DeepCopy
- VladislavTsurikov.AttributeUtility

## Notes

- Automatically handles component creation/deletion
- Supports both same-type and different-type stacks
- Properly manages component lifecycle
- Integrates with existing ComponentStack attributes
- Maintains state across refreshes
- Keyboard shortcuts for rename (Enter/Escape)

## License

Part of the Universal-Toolkit package by Vladislav Tsurikov.
