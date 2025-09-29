# UISystem

**UISystem** is a modular UI architecture for Unity.  
It was originally created inside a company as an experimental solution to unify UI handling and integrate with **Addressables**.  
The system was not fully used in a production game and was not heavily tested in commercial environments, but a dedicated test scene was implemented to validate its functionality.

---

## Features
- **Unified lifecycle** for all UI elements (`Show`, `Hide`, `Destroy`).  
- **UI Handlers** — controller classes that manage windows and their children.  
- **Infinite hierarchy** — children only activate if their parent is active.  
- **Automatic component binding** with `IBindableUIComponent` and `UIComponentBinder`.  
- **Fluent prefab spawning API**:  
  ```csharp
  .Spawn()
    .WithParent(transform)
    .Enable(true)
    .WithName("CustomWindow")
    .Execute(loader, binder, ct);
  ```  
- **Addressables support** — lazy loading/unloading of UI prefabs, with optional load/unload conditions.  
- **Zenject integration** — automatic bind/unbind for UIHandlers and Views.  
- **No strict MVP dependency** — works with MVP, MVVM, or custom approaches.  
- **Back button support** — integration with `UIBackSystem` to close the last opened UI.  
- **Custom activation logic** — define your own rules when a UI element can be shown.  

---

## Why Use This
- Provides a **single standard way** to create and manage UI across the project.  
- Simplifies integration of additional systems (e.g., tutorial pointers, notifications).  
- Reduces boilerplate code compared to manual prefab management.  
- Works seamlessly with **AddressableLoaderSystem** for automatic UI loading/unloading.  
- Supports **Zenject** for dependency injection, improving modularity and testability.  
- Enables advanced UI flows: back navigation, hierarchical activation, lazy child spawning.  

---

## Example
```csharp
public class UIMissionsMainWindowHandler : UnityUIHandler
{
    private MainMissionsWindowView _view;

    public UIMissionsMainWindowHandler(DiContainer container, UIMissionsMainWindowLoader loader)
        : base(container, loader) { }

    protected override async UniTask AfterShowUIHandler(CancellationToken ct, CompositeDisposable d)
    {
        if (_view == null)
            _view = GetUIComponent<MainMissionsWindowView>("MainMissionsWindowView");

        _view.OnCloseClicked
            .Subscribe(_ => Hide().Forget())
            .AddTo(d);
    }

    protected override string GetParentName() => "MainMissionsWindow";
}
```

---

## Example Handlers
- **UIMissionsMainWindowHandler** — root missions window.  
- **ChapterMissionsWindowHandler** — chapter missions.  
- **DailyMissionsWindowHandler** — daily missions.  
- **ProgressMissionsWindowHandler** — mission progress.  

Each child spawns its UI content **only once on the first show**, optimizing performance and memory usage.  

---

## Repository Structure
- **VladislavTsurikov/UISystem** — core modular UI system with Addressables and Zenject integration.  
- **VladislavTsurikov/UIBackSystem** — back button implementation (closes the last opened UI).  
- **VladislavTsurikov/UIRootSystem** — base HUD/UI spawning system for global integration.  
