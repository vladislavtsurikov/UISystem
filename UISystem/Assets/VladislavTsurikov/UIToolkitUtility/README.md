# UIToolkit Utility

UIToolkit-based utilities for Unity Editor development.

## Overview

This package provides UIToolkit implementations of common editor UI components, offering modern, performant alternatives to IMGUI-based solutions.

## Components

### IconStackView

A UIToolkit-based icon grid view component for displaying collections of selectable items with preview images.

**Features:**
- Grid-based icon display with customizable icon sizes
- Multi-selection support (Click, Ctrl+Click, Shift+Click)
- Drag and drop support for reordering and adding items
- Context menu integration
- Custom icon rendering via delegates
- Automatic empty state handling

**Usage Example:**

```csharp
using VladislavTsurikov.UIToolkitUtility.Editor.ElementStack.IconStack;
using VladislavTsurikov.UIToolkitUtility.Runtime.ElementStack.IconStack;

// Create the icon stack view
var iconStackView = new IconStackView(draggable: true);
iconStackView.IconWidth = 80;
iconStackView.IconHeight = 95;
iconStackView.ZeroIconsWarning = "Drag & Drop items here";

// Configure callbacks
iconStackView.AddIconCallback = (obj) => {
    // Handle adding new icon
    return newIcon;
};

iconStackView.IconSelected = (icon) => {
    // Handle icon selection
};

iconStackView.IconMenuCallback = (icon, menu) => {
    // Add context menu items
    menu.AppendAction("Delete", (action) => DeleteIcon(icon));
};

// Set items
iconStackView.SetItems(myList, typeof(MyIconType));

// Add to your UI
rootVisualElement.Add(iconStackView);
```

## Migration from IMGUI

If you're migrating from `IMGUIUtility.Editor.ElementStack.IconStack.IconStackEditor`:

1. Replace `IconStackEditor` with `IconStackView`
2. Instead of calling `OnGUI()`, add the view to your VisualElement hierarchy
3. Update namespace from `VladislavTsurikov.IMGUIUtility.Editor.ElementStack.IconStack` to `VladislavTsurikov.UIToolkitUtility.Editor.ElementStack.IconStack`
4. IconMenuCallback now receives a `DropdownMenu` instead of returning a `GenericMenu`

## Requirements

- Unity 2021.3 or later (UIToolkit support)
- VladislavTsurikov.ComponentStack
- VladislavTsurikov.AttributeUtility

## Assembly Definition

**Name:** `VladislavTsurikov.UIToolkitUtility`
**Platform:** Editor Only

## License

Part of the Universal-Toolkit project.
