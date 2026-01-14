# UIToolkit ReorderableList

A comprehensive UIToolkit-based ReorderableList implementation for Unity Editor, designed to provide a modern alternative to Unity's IMGUI ReorderableList.

## Features

- **Full UIToolkit Integration**: Built entirely with UIToolkit elements for better performance and modern UI
- **Drag and Drop Support**: Visual drag handles with animated reordering
- **Customizable Appearance**: Matches Unity's IMGUI ReorderableList visual style
- **Add/Remove Buttons**: Built-in footer buttons for list manipulation
- **Flexible Item Rendering**: Support for custom item templates
- **Dynamic Height**: Automatic height calculation for complex list items
- **Selection Support**: Single-item selection with visual feedback

## Components

### ReorderableList
Main component that provides the reorderable list functionality.

**Properties:**
- `HeaderTitle` - Title displayed in the header
- `ShowHeader` - Toggle header visibility
- `ShowFooter` - Toggle footer visibility
- `ShowAddButton` - Toggle add button visibility
- `ShowRemoveButton` - Toggle remove button visibility
- `Reorderable` - Enable/disable drag and drop reordering
- `ItemsSource` - The underlying IList data source
- `SelectedIndex` - Currently selected item index

**Callbacks:**
- `MakeItem` - Factory function for creating list item elements
- `BindItem` - Callback for binding data to list items
- `OnAdd` - Custom handler for adding new items
- `OnRemove` - Custom handler for removing items
- `OnReorder` - Callback triggered when items are reordered

### ReorderableListElement
Visual element representing a single list item with drag handle.

**Features:**
- Visual drag handle for reordering
- Content container for custom item content
- Hover effects
- Flexible styling

## Usage Example

```csharp
using VladislavTsurikov.UIToolkitReorderableList.Runtime;
using UnityEngine.UIElements;
using System.Collections.Generic;

public class MyEditorWindow : EditorWindow
{
    private ReorderableList _reorderableList;
    private List<string> _myData;

    public void CreateGUI()
    {
        _myData = new List<string> { "Item 1", "Item 2", "Item 3" };

        _reorderableList = new ReorderableList
        {
            HeaderTitle = "My List",
            ShowHeader = true,
            ShowFooter = true,
            Reorderable = true
        };

        // Define how to create list items
        _reorderableList.MakeItem = () =>
        {
            return new TextField();
        };

        // Define how to bind data to items
        _reorderableList.BindItem = (element, index) =>
        {
            var textField = element as TextField;
            if (textField != null && index < _myData.Count)
            {
                textField.value = _myData[index];
                textField.RegisterValueChangedCallback(evt =>
                {
                    _myData[index] = evt.newValue;
                });
            }
        };

        // Set the data source
        _reorderableList.ItemsSource = _myData;

        rootVisualElement.Add(_reorderableList);
    }
}
```

## Integration with CustomInspector

This assembly is designed to work seamlessly with the CustomInspector system. The `ListFieldDrawer` in CustomInspector automatically uses this ReorderableList for rendering List<T> fields.

## Visual Style

The ReorderableList matches Unity's IMGUI ReorderableList appearance:
- Dark header with bold title
- Alternating row backgrounds
- Selected item highlighting
- Footer with +/- buttons
- Drag handles on each item (when reorderable)

## Requirements

- Unity 2021.3 or newer (UIToolkit support)
- Editor-only assembly

## License

Part of the Universal-Toolkit package by Vladislav Tsurikov.
