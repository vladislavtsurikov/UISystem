# MegaWorld Complex ComponentEditor Migration Notes

This document outlines potential paths to migrate **complex** MegaWorld `ComponentEditor` implementations to CustomInspector by extending the inspector system with reusable drawers/attributes.

## Summary of missing CustomInspector capabilities

The current CustomInspector API supports basic fields, conditional display, and validation attributes, but complex editors need extra UI primitives:

1. **Min/Max slider + synchronized fields**
   - Needed by: `ScaleClampEditor`, `ScaleEditor`, `RandomizeScaleEditor`, `RandomPointEditor`, `MaskOperationsFilterEditor` (Clamp/Remap), `SlopeFilterEditor`.
   - Suggested additions:
     - `MinMaxSliderAttribute(float min, float max)` for float pair fields (e.g., `Vector2` / custom struct).
     - `MinMaxSliderWithFieldsDrawer` that renders:
       - `EditorGUI.MinMaxSlider`
       - two numeric fields bound to the same values
       - optional labels (left/center/right) for fixed tick text (0/2.5/5, 0°/45°/90°, etc.).

2. **Composite field groups with conditional subfields**
   - Needed by: `RotationEditor` (ModifyTransform), `RandomGridEditor`, `HeightFilterEditor`, `SlopeFilterEditor`, `ImageFilterEditor`.
   - Suggested additions:
     - `FoldoutGroup` or `InlineGroup` decorators for grouping related fields.
     - `ShowIf` already exists, but a **compound** condition (AND/OR) would reduce custom editor logic.

3. **Preview/graph UI**
   - Needed by: `ConcavityFilterEditor`, `AspectFilterEditor`, `NoiseFilterEditor`, `HeightNoiseFilterEditor`.
   - Suggested additions:
     - Custom field drawer for `AnimationCurve` that can invoke a callback to regenerate textures.
     - A `PreviewTextureAttribute` + drawer to render a small texture field with optional height/width control.

4. **Context-sensitive warnings / help boxes**
   - Needed by: `TexturesFilterEditor`, `ImageFilterEditor`, `SlopeFilterEditor`.
   - Suggested additions:
     - `HelpBoxAttribute` with condition callback or `ShowIf` + message field support.
     - `RequiresTerrainAttribute` (decorator) that shows a warning when `Terrain.activeTerrain == null` or `drawInstanced == false`.

5. **List index–dependent UI**
   - Needed by: `HeightFilterEditor`, `AspectFilterEditor`, `SlopeFilterEditor` (hide BlendMode for index 0).
   - Suggested additions:
     - Extend `ElementEditor` or CustomInspector stack integration to pass the element index to drawers.
     - Alternatively: add a `HideIfFirstElementAttribute` used by the list drawer.

---

## Candidate complex editors to migrate first

These have minimal custom logic beyond min/max slider UI and could be migrated after adding a MinMaxSlider drawer:

- `ScaleClampEditor` (TransformElementSystem)
- `ScaleEditor` (TransformElementSystem)
- `RandomizeScaleEditor` (BrushModifyTool)
- `RandomPointEditor` (ScatterSystem)
- `MaskOperationsFilterEditor` (Clamp/Remap modes)
- `SlopeFilterEditor` (min/max slope with labels)

**Rationale:** their behavior is mostly standard UI widgets + basic min/max synchronization.

---

## Higher complexity editors (keep for now)

These editors rely on terrain, previews, or dynamic layouts and likely need dedicated drawers or decorators:

- `TexturesFilterEditor`
- `NoiseFilterEditor`
- `HeightNoiseFilterEditor`
- `ConcavityFilterEditor`
- `AspectFilterEditor`
- `ImageFilterEditor`
- `RandomGridEditor`

---

## Suggested implementation order (if you want to migrate)

1. Add a generic **MinMaxSlider drawer** for `Vector2` or a custom `MinMaxFloat` struct.
2. Add **label presets** to the drawer (e.g., `MinMaxSliderLabelPreset.SlopeDegrees`).
3. Add **list index context** to the ReorderableList CustomInspector integration.
4. Add **HelpBox** and **PreviewTexture** decorators.
5. Replace complex editors incrementally, starting with Scale/Clamp/RandomPoint.

---

## Notes

- This approach keeps the CustomInspector system extensible by adding *generic* drawers, not MegaWorld-specific code.
- Once these drawers exist, each complex editor can be replaced by a combination of attributes + structured field types.
