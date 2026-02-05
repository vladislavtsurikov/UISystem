# CustomInspector Technical Architecture

This document describes how the CustomInspector system works internally and how the main classes interact.

## Goals

- Provide a pluggable inspector pipeline for IMGUI and UI Toolkit.
- Resolve field drawers by exact attribute sets and field types.
- Support decorator drawers (visual add-ons), value processors (post-processing), and field logic/style processors.
- Cache resolved metadata per inspected type for performance.

## High-Level Flow

1. An inspector implementation (IMGUI or UI Toolkit) requests processed fields for a target object.
2. Processed fields are resolved and cached per target type.
3. For each field:
   - Decorators are created and drawn first.
   - A field drawer is selected and used to render/edit the field.
   - Value processors apply post-processing to the new value.
4. If no drawer is found, the system falls back to recursive drawing of nested objects.

## Core Components

### InspectorContext

File: `Assets/VladislavTsurikov/CustomInspector/Editor/Core/InspectorContext.cs`

- Provides per-draw context via a thread-local stack.
- Each frame of context stores:
  - `Target`: current object being drawn.
  - `Field`: current `FieldInfo`.
  - `ElementIndex`: optional list element index.
- Drawers and processors can query `InspectorContext.Current` to access the current field/target.

### InspectorFieldsDrawer

File: `Assets/VladislavTsurikov/CustomInspector/Editor/Core/InspectorFieldsDrawer.cs`

Responsibilities:
- Enumerate serializable fields and build cached `ProcessedField` entries per target type.
- Apply field rules:
  - Visibility (`HideInInspector`, `ShowIf`, `HideIf`)
  - State (`DisableIf`, `ReadOnly`)
  - Style (`GUIColor`)
  - Order, tooltip, etc.
- Resolve:
  - Field drawer (via `FieldDrawerResolver`).
  - Decorators (via `DecoratorDrawerResolver`).
  - Value processors (via `FieldValueProcessorResolver`).

`ProcessedField` contains:
- `Field`: `FieldInfo`
- `FieldName`: resolved label
- `Drawer`: resolved field drawer
- `Decorators`: resolved decorator drawers
- `VisibilityProcessors`: resolved visibility processors
- `StateProcessors`: resolved state processors
- `StyleProcessors`: resolved style processors
- `ValueProcessors`: resolved processors
- `Tooltip`
- `Value`: current field value

### FieldDrawerMatcher / FieldDrawerResolver

Files:
- `Assets/VladislavTsurikov/CustomInspector/Editor/Core/FieldDrawerMatcher.cs`
- `Assets/VladislavTsurikov/CustomInspector/Editor/Core/FieldDrawerResolver.cs`

Key ideas:
- Drawers are matched using `FieldInfo` (not just type).
- Each matcher can declare `AttributeTypes` (a set of attribute types it requires).
- Resolution rules:
  1. Build the field's "drawer attribute set" by checking only attributes declared by matchers.
  2. Find a matcher whose `AttributeTypes` exactly match that set and `CanDraw(field)` is true.
  3. If no exact match, fall back to the first matcher with empty `AttributeTypes`.

This enables override drawers for specific attribute combinations while keeping a clean fallback.

### DecoratorDrawerMatcher / DecoratorDrawerResolver

Files:
- `Assets/VladislavTsurikov/CustomInspector/Editor/Core/DecoratorDrawerMatcher.cs`
- `Assets/VladislavTsurikov/CustomInspector/Editor/Core/DecoratorDrawerResolver.cs`

- Decorators are matched per attribute.
- Each decorator is initialized with its attribute instance.
- Decorators draw visual content before the field itself.

### FieldVisibilityProcessorMatcher / FieldVisibilityProcessorResolver

Files:
- `Assets/VladislavTsurikov/CustomInspector/Editor/Core/FieldProcessing/FieldVisibilityProcessorMatcher.cs`
- `Assets/VladislavTsurikov/CustomInspector/Editor/Core/FieldProcessing/FieldVisibilityProcessorResolver.cs`
- `Assets/VladislavTsurikov/CustomInspector/Editor/Core/FieldProcessing/FieldVisibilityProcessor.cs`

- Processors decide whether a field is visible.
- All matching processors are applied in attribute enumeration order.
- Example processors:
  - `ShowIfVisibilityProcessor`
  - `HideIfVisibilityProcessor`
  - `HideInInspectorVisibilityProcessor`

### FieldStateProcessorMatcher / FieldStateProcessorResolver

Files:
- `Assets/VladislavTsurikov/CustomInspector/Editor/Core/FieldProcessing/FieldStateProcessorMatcher.cs`
- `Assets/VladislavTsurikov/CustomInspector/Editor/Core/FieldProcessing/FieldStateProcessorResolver.cs`
- `Assets/VladislavTsurikov/CustomInspector/Editor/Core/FieldProcessing/FieldStateProcessor.cs`

- Processors modify `FieldState` (enabled/read-only).
- All matching processors are applied in attribute enumeration order.
- Example processors:
  - `DisableIfFieldStateProcessor`
  - `ReadOnlyFieldStateProcessor`

### FieldStyleProcessorMatcher / FieldStyleProcessorResolver

Files:
- `Assets/VladislavTsurikov/CustomInspector/Editor/Core/FieldProcessing/FieldStyleProcessorMatcher.cs`
- `Assets/VladislavTsurikov/CustomInspector/Editor/Core/FieldProcessing/FieldStyleProcessorResolver.cs`
- `Assets/VladislavTsurikov/CustomInspector/Editor/Core/FieldProcessing/FieldStyleProcessor.cs`

- Processors modify `FieldStyle` (colors, etc.).
- All matching processors are applied in attribute enumeration order.
- Example processors:
  - `GUIColorFieldStyleProcessor`

### FieldValueProcessorMatcher / FieldValueProcessorResolver

Files:
- `Assets/VladislavTsurikov/CustomInspector/Editor/Core/FieldValueProcessorMatcher.cs`
- `Assets/VladislavTsurikov/CustomInspector/Editor/Core/FieldValueProcessorResolver.cs`
- `Assets/VladislavTsurikov/CustomInspector/Editor/Core/FieldValueProcessor.cs`

- Processors post-process the value returned by a drawer.
- All matching processors are applied in attribute enumeration order.
- Example processors:
  - `MinValueProcessor`
  - `MaxValueProcessor`

## IMGUI Pipeline

Files:
- `Assets/VladislavTsurikov/CustomInspector/Editor/IMGUI/IMGUIInspectorFieldsDrawer.cs`
- `Assets/VladislavTsurikov/CustomInspector/Editor/IMGUI/IMGUIFieldDrawer.cs`

Steps per field:
1. Push `InspectorContext`.
2. Draw all decorators (each can contribute height).
3. Apply state/style processors (disabled, GUI color).
4. Draw field using resolved `IMGUIFieldDrawer`.
5. Apply value processors to the returned value.
6. If value changed, assign it to the field.
7. If no drawer exists, recurse into nested object drawing.

## UI Toolkit Pipeline

Files:
- `Assets/VladislavTsurikov/CustomInspector/Editor/UIToolkit/UIToolkitInspectorFieldsDrawer.cs`
- `Assets/VladislavTsurikov/CustomInspector/Editor/UIToolkit/UIToolkitFieldDrawer.cs`

Steps per field:
1. Push `InspectorContext`.
2. Create decorator elements and add them to the container.
3. Apply value processors to the current value before rendering.
4. Create a `VisualElement` using the resolved drawer.
5. Apply state/style processors (enabled state, background color).
6. Register a value-changed callback that:
   - Pushes `InspectorContext`.
   - Applies processors.
   - Writes back to the field.
7. If no drawer exists, recurse into nested object drawing.

## Attribute-Based Override Drawers

The system supports exact attribute set matching for drawers. Examples:

- Range/MinMax slider drawers are implemented as override drawers:
  - `RangeIntFieldDrawer`, `RangeFloatFieldDrawer`
  - `MinMaxSliderIntFieldDrawer`, `MinMaxSliderFloatFieldDrawer`
  - `MinMaxSliderVector2FieldDrawer`, `MinMaxSliderVector3FieldDrawer`

Each of these matchers declares `AttributeTypes` so they only match when the
field has exactly those attributes (among attributes declared by matchers).

## Value Processors

Example processors:
- `MinValueProcessor` clamps numeric values to a minimum.
- `MaxValueProcessor` clamps numeric values to a maximum.

Both are applied without priority ordering; if both attributes are present, both
processors run.

## Recursion and Complex Types

If no field drawer exists, `RecursiveFieldsDrawer` is used to render nested
objects. This enables complex types to be drawn as a group of child fields.

## Caching

`InspectorFieldsDrawer` caches resolved `ProcessedField` entries per target type:
- Drawer matching
- Decorator list
- Processor list
- Field metadata (labels, tooltips)

This reduces reflection overhead during repeated draws.

## Extending CustomInspector

### Add a new field drawer
1. Create a drawer class derived from `IMGUIFieldDrawer` or `UIToolkitFieldDrawer`.
2. Create a matcher derived from `FieldDrawerMatcher<TDrawer>`.
3. Implement `CanDraw(FieldInfo field)`.
4. Optionally declare `AttributeTypes` for exact-attribute matching.

### Add a decorator
1. Create a decorator drawer derived from `IMGUIDecoratorDrawer` or `UIToolkitDecoratorDrawer`.
2. Create a matcher derived from `DecoratorDrawerMatcher<TDrawer>`.
3. Implement `CanDraw(Attribute attribute)`.

### Add a value processor
1. Create a processor derived from `FieldValueProcessor`.
2. Create a matcher derived from `FieldValueProcessorMatcher`.
3. Implement `CanProcess(Attribute attribute)`.
4. Implement `Process(FieldInfo field, object target, object value)`.

### Add a visibility processor
1. Create a processor derived from `FieldVisibilityProcessor`.
2. Create a matcher derived from `FieldVisibilityProcessorMatcher`.
3. Implement `CanProcess(Attribute attribute)`.
4. Implement `IsVisible(FieldInfo field, object target)`.

### Add a state processor
1. Create a processor derived from `FieldStateProcessor`.
2. Create a matcher derived from `FieldStateProcessorMatcher`.
3. Implement `CanProcess(Attribute attribute)`.
4. Implement `Apply(FieldInfo field, object target, FieldState state)`.

### Add a style processor
1. Create a processor derived from `FieldStyleProcessor`.
2. Create a matcher derived from `FieldStyleProcessorMatcher`.
3. Implement `CanProcess(Attribute attribute)`.
4. Implement `Apply(FieldInfo field, object target, FieldStyle style)`.

## Related Files and Folders

- Core:
  - `Assets/VladislavTsurikov/CustomInspector/Editor/Core/`
- Field processing:
  - `Assets/VladislavTsurikov/CustomInspector/Editor/Core/FieldProcessing/`
- IMGUI:
  - `Assets/VladislavTsurikov/CustomInspector/Editor/IMGUI/`
- UI Toolkit:
  - `Assets/VladislavTsurikov/CustomInspector/Editor/UIToolkit/`
- Field processors:
  - `Assets/VladislavTsurikov/CustomInspector/Editor/FieldProcessors/`
- Runtime attributes:
  - `Assets/VladislavTsurikov/CustomInspector/Runtime/DecoratorAttributes/`
