# EntityDataActionFramework — Common Patterns

This document collects common architectural patterns used in the project.

## Pattern: Read data, update UI only
- Action depends on data A.
- Action writes to UI elements only.
- Safe, no recursion risk.

## Pattern: Derived data
- Action depends on input data and writes derived data.
- Ensure derived data is a different ComponentData type to avoid loops.

## Pattern: Split responsibilities
- Keep one action for computing state.
- Keep another action for applying UI visuals.

## Pattern: List panel rendering
- Use ListPanelAction<TItem, TView> as the base.
- Action pulls a list and maps items to views.
- Disable unused views after rendering.

## Pattern: UI text formatting
- Keep formatting in action, data only stores raw values.
- Use localization layer when available.

## Pattern: Shared utilities
- Reusable data/actions go into EntityDataAction.Shared.
- Feature-specific actions stay in _ProjectDiamondDogs.
