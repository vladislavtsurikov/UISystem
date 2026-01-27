# EntityDataActionFramework — Best Practices

These rules should be applied consistently to avoid regressions and recursion.

## 1) Keep ComponentData minimal
- One purpose per data class.
- Avoid large structs or unrelated fields.

## 2) Idempotent setters
- Always early-return if the value is unchanged.
- For lists, compare contents before assigning.

## 3) Avoid action loops
- Do not include a data type in RunOnDirtyData if the action writes that data.
- If needed, split into two actions.

## 4) Handle missing data safely
- If external data is missing, return early.
- Do not constantly write default values on missing data.

## 5) Keep UI logic in actions
- Avoid logic inside MonoBehaviour UI scripts.
- Actions should be the source of truth for UI state changes.

## 6) Use Shared for reusable components
- Reusable data/actions go to:
  `Assets/Plugins/VladislavTsurikov/EntityDataAction.Shared/Runtime/`
- Feature-specific code stays under `_ProjectDiamondDogs`.

## 7) Prefer small, composable actions
- One responsibility per action.
- Easier to reason about and test.

## 8) Be explicit about subscriptions
- Minimize RunOnDirtyData dependencies.
- Avoid broad subscriptions that re-trigger frequently.
