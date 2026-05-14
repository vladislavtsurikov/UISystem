# UISystem

**UISystem** is a modular UI architecture for Unity built around `UIPresenter` classes, a shared presenter tree, Addressables-based loading, view binding, and dependency-injection friendly composition.

It gives a project one consistent way to create, show, hide, compose, and destroy UI: HUD elements, full-screen screens, popups, contextual panels, floating messages, tabs, and dynamically spawned child views can all participate in the same lifecycle and hierarchy rules.

The package started as an internal experiment for unifying UI flows and integrating UI prefabs with **Addressables**, **Zenject**, scene composition, and reusable root layers. It includes test scenes and mission-style sample presenters, but production projects should still validate their own loading, memory, DI, and navigation rules.

## Why Use It

- **One UI pipeline** - route HUDs, screens, popups, panels, and nested UI through `UIPresenterManager` instead of scattering manual prefab management across gameplay code.
- **Predictable lifecycle** - every presenter follows `Initialize -> BeforeShow -> OnShow -> AfterShow -> BeforeHide -> OnHide -> AfterHide -> Destroy`.
- **Hierarchy by code** - child presenters declare parents with `[UIParent(typeof(...))]`; dynamic children use `[DynamicUIPresenterChild]`.
- **Addressables-first UI** - UGUI prefabs and UI Toolkit layouts can be loaded lazily when the presenter is shown, then unloaded when destroyed.
- **Automatic view binding** - spawned UGUI components and UI Toolkit elements can be resolved from presenters by binding id.
- **Scene-aware composition** - scene filters let the system remove scene-local presenters and compose the UI needed by the next scene.
- **DI friendly** - presenters and views are registered in the dependency container with stable ids.
- **Pattern agnostic** - use MVP, MVVM, presenter-only UI, or your own style. UISystem provides lifecycle and composition, not a forced app pattern.
- **UGUI and UI Toolkit** - the same presenter model supports `UnityUIPresenter` and `UIToolkitUIPresenter`.

## Current Architecture

### `UIPresenter`

`UIPresenter` is the base class for UI behavior. It owns lifecycle hooks, parent/child relationships, dynamic children, active state, and view resolution.

Key API:

- `Show(CancellationToken)` and `Hide(CancellationToken)`
- `InitializeUIPresenter(...)`
- `BeforeShowUIPresenter(...)`, `OnShowUIPresenter(...)`, `AfterShowUIPresenter(...)`
- `BeforeHideUIPresenter(...)`, `OnHideUIPresenter(...)`, `AfterHideUIPresenter(...)`
- `DestroyUIPresenter(...)`
- `GetView<TView>(string bindingId, int index = 0)`
- `CreateDynamicChild<TPresenter>(string instanceKey, bool showAutomatically = false, ...)`
- `DestroyDynamicChild<TPresenter>(string instanceKey, bool unload, ...)`

`UIPresenter` also exposes global lifecycle events such as `OnUIPresenterBeforeShow`, `OnUIPresenterAfterShow`, and `OnUIPresenterDestroyed`, which are useful for navigation, analytics, overlays, and debug tooling.

### `UIPresenterManager`

`UIPresenterManager` creates presenters, attaches them to parents, registers them in DI, applies `FilterAttribute` rules, and cleans up presenters that no longer match the active filters.

The manager gets its hierarchy from:

- `NodeTreeAsset`, generated from the editor menu `Tools/UISystem/Generate Node Tree`; or
- runtime reflection over all classes derived from `UIPresenter`.

### `UINavigator`

`UINavigator` is a small static facade for feature code:

```csharp
await UINavigator.Show<UpgradeWindowPresenter>(ct);
await UINavigator.Hide<UpgradeWindowPresenter>(ct);
```

It resolves the presenter from the dependency container and uses `[UIParent]` when the presenter is registered under a parent type.

## Presenter Types

### UGUI: `UnityUIPresenter`

`UnityUIPresenter` spawns a `GameObject` prefab through a `PrefabAssetLoader`. The spawned root is cached, activated on show, deactivated on hide, and destroyed on presenter destroy.

```csharp
SpawnedRoot = await UnityCanvasSpawnOperation.Spawn()
    .WithParent(parentTransform)
    .Enable(true)
    .WithName(SpawnedRootName)
    .Execute(Loader, ComponentBinder, cancellationToken);
```

By default, a child `UnityUIPresenter` is spawned under its parent presenter root. Override `GetSpawnParentTransform()` when a presenter needs a custom container.

### UI Toolkit: `UIToolkitUIPresenter`

`UIToolkitUIPresenter` loads a UI Toolkit layout through `UIToolkitLayoutLoader`, attaches it to a parent `VisualElement`, stretches it to the parent, and toggles visibility through `display`.

For nested UI Toolkit presenters, the parent container can be supplied by:

- overriding `ParentContainerName`; or
- passing a container id to `[UIParent(typeof(ParentPresenter), "ContainerId")]`.

If the presenter has no loader, it can use the parent binding context and resolve an existing root/container instead of spawning a new layout.

## View Binding

UGUI components bind by implementing `IBindableView`:

```csharp
public sealed class SampleWindowView : MonoBehaviour, IBindableView
{
    public string BindingId => nameof(SampleWindowView);
}
```

`UnityUIComponentBinder` finds all `IBindableView` components under the spawned prefab and registers them in DI. Presenters can then resolve views:

```csharp
View = GetView<SampleWindowView>(nameof(SampleWindowView));
```

When multiple views share the same type and binding id, use the optional `index` argument:

```csharp
var secondButton = GetView<MenuButtonView>("MenuButton", 1);
```

UI Toolkit uses the same idea through `UIToolkitElementBinder`: elements bind by `IBindableView.BindingId` when they implement `IBindableView`, otherwise by `VisualElement.name`.

## Hierarchy

Static child presenters declare their parent:

```csharp
[UIParent(typeof(BattleHUDRootPresenter))]
public sealed class OpenUpgradeHUDButtonPresenter : UIToolkitUIPresenter
{
}
```

Dynamic child presenters are excluded from the static tree and are created manually from a parent:

```csharp
[DynamicUIPresenterChild]
public sealed class InventorySlotPresenter : UnityUIPresenter
{
}

await CreateDynamicChild<InventorySlotPresenter>(
    instanceKey: slotId,
    showAutomatically: true,
    cancellationToken: ct);
```

`UIPresenterChildrenModule` initializes child presenters only after their parent is shown, hides active children when the parent hides, and destroys all children when the parent is destroyed.

For tab-like flows, `SingleActiveChildPresenterModule` keeps only one direct child active at a time.

## Scene Composition

When `AddressableLoaderSystem` integration is available, UISystem can compose scene-specific UI through filters:

- `SceneUICompositionService.ComposeScene(sceneName)` adds a filter for presenters marked with matching `SceneFilterAttribute`.
- `RemoveScenePresenters()` removes scene-local presenters while keeping global presenters.
- `SceneCompositionService` wraps built-scene and Addressable-scene loading, resource loading, scene activation, and UI composition.

This gives projects a single place to decide which UI belongs to the current scene and when scene UI should be created or removed.

## Examples from ShooterUpgradePrototype

These examples are adapted from the runtime UI layer in [ShooterUpgradePrototype](https://github.com/vladislavtsurikov/ShooterUpgradePrototype). They show how UISystem is used in a real gameplay prototype with UI Toolkit layouts, Addressables loaders, scene filters, DI, reactive bindings, and dynamic child presenters. Some feature-specific methods are abbreviated so the examples stay focused on UISystem usage.

### Scene HUD Root

The root presenter loads the Battle HUD layout into the `HUD` slot of the global UI root. It is created only for the `Battle` scene and shows itself immediately after initialization.

```csharp
using System.Threading;
using Cysharp.Threading.Tasks;
using UniRx;
using UIRootSystem.Runtime;
using VladislavTsurikov.AddressableLoaderSystem.Runtime.Core;
using VladislavTsurikov.UISystem.Runtime.Core;
using VladislavTsurikov.UISystem.Runtime.UIToolkitIntegration;

[SceneFilter("Battle")]
[UIParent(typeof(Root), RootSlots.HUD)]
public sealed class BattleHUDRootPresenter : UIToolkitUIPresenter
{
    public BattleHUDRootPresenter(BattleHUDLayoutLoader loader)
        : base(loader)
    {
    }

    protected override async UniTask InitializeUIPresenter(
        CancellationToken cancellationToken,
        CompositeDisposable disposables) => await Show(cancellationToken);

    protected override string SpawnedRootName => "ShooterUpgradePrototypeBattleHUD";
}
```

### HUD Button Opens a Screen

This presenter does not load its own layout. It is a child of `BattleHUDRootPresenter`, resolves an already-bound `OpenUpgradeHUDButtonView`, and opens the upgrade window through `UINavigator`.

```csharp
using System.Threading;
using Cysharp.Threading.Tasks;
using UniRx;
using VladislavTsurikov.AddressableLoaderSystem.Runtime.Core;
using VladislavTsurikov.UISystem.Runtime.Core;
using VladislavTsurikov.UISystem.Runtime.UIToolkitIntegration;

[SceneFilter("Battle")]
[UIParent(typeof(BattleHUDRootPresenter))]
public sealed class OpenUpgradeHUDButtonPresenter : UIToolkitUIPresenter
{
    private OpenUpgradeHUDButtonView _view;

    protected override UniTask InitializeUIPresenter(
        CancellationToken cancellationToken,
        CompositeDisposable disposables)
    {
        _view = GetView<OpenUpgradeHUDButtonView>(nameof(OpenUpgradeHUDButtonView));

        _view.OnClicked
            .Subscribe(_ => UINavigator.Show<UpgradeWindowPresenter>(cancellationToken).Forget())
            .AddTo(disposables);

        return UniTask.CompletedTask;
    }
}
```

### Reactive HUD Presenter

This HUD presenter subscribes to gameplay state and keeps a bound view updated. The subscriptions are attached to the presenter's disposable lifetime.

```csharp
using System.Threading;
using Cysharp.Threading.Tasks;
using UniRx;
using VladislavTsurikov.AddressableLoaderSystem.Runtime.Core;
using VladislavTsurikov.UISystem.Runtime.Core;
using VladislavTsurikov.UISystem.Runtime.UIToolkitIntegration;

[SceneFilter("Battle")]
[UIParent(typeof(BattleHUDRootPresenter))]
public sealed class PlayerHealthHUDPresenter : UIToolkitUIPresenter
{
    private const string HealthStatId = "HP";

    private readonly PlayerStatsService _playerStatsService;
    private PlayerHealthHUDView _view;

    public PlayerHealthHUDPresenter(PlayerStatsService playerStatsService)
    {
        _playerStatsService = playerStatsService;
    }

    protected override UniTask InitializeUIPresenter(
        CancellationToken cancellationToken,
        CompositeDisposable disposables)
    {
        _view = GetView<PlayerHealthHUDView>(nameof(PlayerHealthHUDView));

        _playerStatsService.GetValueProperty(HealthStatId)
            .Subscribe(_ => RenderHealth())
            .AddTo(disposables);

        _playerStatsService.GetLevelProperty(HealthStatId)
            .Subscribe(_ => RenderHealth())
            .AddTo(disposables);

        RenderHealth();
        return UniTask.CompletedTask;
    }

    private void RenderHealth()
    {
        float currentValue = _playerStatsService.GetCurrentValue(HealthStatId);
        float maxValue = _playerStatsService.GetCurrentMaxValue(HealthStatId);

        _view.SetHealthText(currentValue, maxValue);
    }
}
```

### Screen Presenter with Show-Time Bindings

The upgrade window is loaded as a screen, disables gameplay input while visible, creates dynamic stat rows, and clears show-specific bindings when hidden.

```csharp
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UniRx;
using UIRootSystem.Runtime;
using VladislavTsurikov.AddressableLoaderSystem.Runtime.Core;
using VladislavTsurikov.UISystem.Runtime.Core;
using VladislavTsurikov.UISystem.Runtime.UIToolkitIntegration;

[SceneFilter("Battle")]
[UIParent(typeof(ScreenPresenter))]
public sealed class UpgradeWindowPresenter : UIToolkitUIPresenter
{
    private readonly GameplayInputBlocker _gameplayInputBlocker;
    private readonly PlayerStatsService _playerStatsService;
    private readonly IReadOnlyList<string> _upgradeStatIds;
    private readonly Dictionary<string, int> _draftLevels = new();
    private readonly SerialDisposable _showBindings = new();

    private UpgradeWindowView _view;

    public UpgradeWindowPresenter(
        UpgradeWindowLayoutLoader loader,
        PlayerInputActions playerInputActions,
        PlayerStatsService playerStatsService) : base(loader)
    {
        _gameplayInputBlocker = new GameplayInputBlocker(playerInputActions);
        _playerStatsService = playerStatsService;
        _upgradeStatIds = _playerStatsService.GetUpgradeWindowStatIds();
    }

    protected override string SpawnedRootName => "ShooterUpgradePrototypeUpgradeWindow";

    protected override async UniTask AfterShowUIPresenter(
        CancellationToken cancellationToken,
        CompositeDisposable disposables)
    {
        _gameplayInputBlocker.DisableGameplayInput();
        _view = GetView<UpgradeWindowView>(nameof(UpgradeWindowView));

        CompositeDisposable showDisposables = new();
        _showBindings.Disposable = showDisposables;

        _view.OnCloseClicked
            .Subscribe(_ => UINavigator.Hide<UpgradeWindowPresenter>(CancellationToken.None).Forget())
            .AddTo(showDisposables);

        _view.OnApplyClicked
            .Subscribe(_ => ApplyDraftLevels())
            .AddTo(showDisposables);

        await CreateStatRows(cancellationToken);
        Refresh();
    }

    protected override UniTask AfterHideUIPresenter(
        CancellationToken cancellationToken,
        CompositeDisposable disposables)
    {
        _showBindings.Disposable = Disposable.Empty;
        _gameplayInputBlocker.EnableGameplayInput();
        return UniTask.CompletedTask;
    }

    private async UniTask CreateStatRows(CancellationToken cancellationToken)
    {
        foreach (string statId in _upgradeStatIds)
        {
            UpgradeStatRowPresenter row = GetDynamicChild<UpgradeStatRowPresenter>(statId);

            if (row == null)
            {
                row = await CreateDynamicChild<UpgradeStatRowPresenter>(
                    statId,
                    showAutomatically: true,
                    cancellationToken);

                row.UpgradeRequested += AddDraftLevel;
            }
        }
    }
}
```

### Dynamic UI Toolkit Row

Dynamic child presenters use `InstanceKey` to distinguish repeated UI instances. Here every stat row is spawned into the `rowsContainer` element of the parent upgrade window.

```csharp
using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine.UIElements;
using VladislavTsurikov.AddressableLoaderSystem.Runtime.Core;
using VladislavTsurikov.UISystem.Runtime.Core;
using VladislavTsurikov.UISystem.Runtime.UIToolkitIntegration;

[SceneFilter("Battle")]
[DynamicUIPresenterChild]
public sealed class UpgradeStatRowPresenter : UIToolkitUIPresenter
{
    private UpgradeStatRowView _view;

    public event Action<string> UpgradeRequested;

    protected override string ParentContainerName => "rowsContainer";
    protected override string SpawnedRootName => $"ShooterUpgradePrototypeUpgradeStatRow:{InstanceKey}";

    public UpgradeStatRowPresenter(UpgradeStatRowLayoutLoader loader)
        : base(loader)
    {
    }

    protected override UniTask AfterShowUIPresenter(
        CancellationToken cancellationToken,
        CompositeDisposable disposables)
    {
        if (_view != null)
        {
            return UniTask.CompletedTask;
        }

        _view = GetView<UpgradeStatRowView>(nameof(UpgradeStatRowView));

        _view.OnUpgradeClicked
            .Subscribe(_ => UpgradeRequested?.Invoke(InstanceKey))
            .AddTo(disposables);

        _view.style.position = Position.Relative;
        return UniTask.CompletedTask;
    }
}
```

## Optional Integrations and Define Symbols

The package includes editor auto-define rules that enable integration code when related packages are present:

- `UI_SYSTEM_ADDRESSABLE_LOADER_SYSTEM` - enables AddressableLoaderSystem integration, prefab loaders, scene composition, and UI Toolkit layout loaders.
- `UI_SYSTEM_ZENJECT` - enables Zenject installers.
- `UI_SYSTEM_UNIRX` - enables UniRx-dependent presenter code.

The most common package stack is:

- **AddressableLoaderSystem** - resource loading, filters, reference tracking, and unloading.
- **UIRootSystem** - shared root layers such as HUD, Screens, and Popups.
- **UIBackSystem** - back-button flow for closing active UI.
- **ZenjectUtility** - Zenject scene-loading helpers.
- **SceneManagerTool** - scene composition workflow.

## Installation

Add the package to Unity Package Manager from Git:

```text
https://github.com/vladislavtsurikov/UISystem.git
```

Package name:

```text
com.vladislavtsurikov.uisystem
```

## Repository Structure

- `Runtime/Core` - presenter lifecycle, hierarchy, navigation, filters, view binding, and dynamic children.
- `Runtime/UnityUIIntegration` - UGUI prefab spawning and component binding.
- `Runtime/UIToolkitIntegration` - UI Toolkit layout loading, element binding, and VisualElement spawning.
- `Runtime/AddressableLoaderSystemIntegration` - prefab loaders and scene composition helpers.
- `Runtime/ZenjectIntegration` - presenter manager installer.
- `Editor` - auto-define rules and NodeTree generation tooling.
- `Tests` - test scenes and mission UI sample presenters.

## Notes

- UISystem is infrastructure, not a finished visual UI kit.
- The current implementation is centered on `UIPresenter`; older docs may refer to `UIHandler`, which was the previous terminology.
- The architecture is intentionally presentation-pattern agnostic. A presenter can act as an MVP presenter, a view-model coordinator, or a lightweight lifecycle wrapper.
- Validate Addressables groups, DI setup, scene filters, and unload behavior in the target project before relying on the package in production.
