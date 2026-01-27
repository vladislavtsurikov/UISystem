# EntityDataActionFramework — User Guide

This guide explains how to use EntityDataActionFramework in this project.
It focuses on concepts, attributes, and a practical Entity/Action flow.

## What it is
A data-driven UI logic framework:
- ComponentData stores state.
- EntityAction reacts to state changes.
- Entity groups data + actions.

The main idea: UI state is driven by data. Actions read data and update UI or
other data. Actions are triggered by data changes, not by direct UI code.

## Core concepts

### ComponentData
Small, focused state containers. Setters must be idempotent and call MarkDirty
only when the value actually changes.

MarkDirty() informs the framework that this data instance changed. It is the
signal that triggers actions subscribed via RunOnDirtyData. If you update
fields without MarkDirty(), dependent actions will not re-run and UI will not
refresh.

Example ComponentData (from project):
```csharp
using OdinSerializer;
using Plugins.VladislavTsurikov.EntiryDataAction.Runtime;
using VladislavTsurikov.CustomInspector.Runtime;
using VladislavTsurikov.ReflectionUtility;

namespace VladislavTsurikov.EntityDataActionFramework
{
    [Name("UI/Common/RarityData")]
    public sealed class RarityData : ComponentData
    {
        [OdinSerialize]
        [Min(0)]
        private int _rarity;

        public int Rarity
        {
            get => _rarity;
            set
            {
                if (_rarity == value)
                {
                    return;
                }

                _rarity = value;
                MarkDirty();
            }
        }
    }
}
```

### EntityAction
Logic that reacts to data. Typical responsibilities:
- Read ComponentData.
- Update UI elements or write derived ComponentData.

Lifecycle methods:
- OnFirstSetupComponent(object[] setupData)
  Runs once when the action is created. Use it to cache DI dependencies or
  resolve services. Avoid heavy logic here.

- OnSetupComponent(object[] setupData)
  Runs when the action is set up. Useful if you need to reset state or respond
  to setup data changes.

- Run(CancellationToken token)
  The main execution method. Called when dependencies are dirty (or on setup,
  depending on the framework). Should be fast and side-effect safe.

Example Action (from project):
```csharp
using System.Threading;
using Cysharp.Threading.Tasks;
using OdinSerializer;
using TMPro;
using UnityDI;
using UnityEngine;
using VladislavTsurikov.ReflectionUtility;

namespace VladislavTsurikov.EntityDataActionFramework
{
    [RunOnDirtyData(typeof(BasicWeaponDefData))]
    [RequiresData(typeof(BasicWeaponDefData))]
    [Name("UI/Gunship/SetTierCountAction")]
    public sealed class SetTierCountAction : EntityAction
    {
        [OdinSerialize]
        private TextMeshProUGUI _tierText;

        protected override void OnFirstSetupComponent(object[] setupData)
        {
            DI.BuildUp(this);
        }

        protected override UniTask<bool> Run(CancellationToken token)
        {
            if (_tierText == null)
            {
                Debug.LogWarning($"{GetType().Name}: _tierText is null");
                return UniTask.FromResult(true);
            }

            BasicWeaponDefData weaponDefData = Get<BasicWeaponDefData>();

            _tierText.text = weaponDefData.BasicWeaponDef.Tier.ToStringFast();
            return UniTask.FromResult(true);
        }
    }
}
```

### Entity
Entity wires data + actions together. It is the composition root for a UI
widget or screen.

You can use Entity in two ways:
- Add Entity as a component in the scene or prefab and configure ComponentData
  and EntityAction manually in the Inspector.
- Inherit from Entity and define the setup in code.

Entity virtual methods:
- ComponentDataTypesToCreate()
  Returns a list of ComponentData types to create and attach.
- ActionTypesToCreate()
  Returns a list of EntityAction types to create and attach.

## Entity usage notes

### Inheriting from Entity
When you inherit from Entity and override the creation methods, the framework
controls the ComponentData/EntityAction list. In this mode, the Inspector list
is locked: adding/removing/reordering entries is blocked to prevent divergence
from the code-defined setup.

Example Entity (from project):
```csharp
using System;
using UI.Gunship.ComponentsData;
using VladislavTsurikov.EntityDataActionFramework;

namespace UI.Gunship
{
    public sealed class WeaponSlotCard : Entity
    {
        protected override Type[] ComponentDataTypesToCreate()
        {
            return new[]
            {
                typeof(WeaponSlotData),
                typeof(ButtonData),
                typeof(BasicWeaponDefData),
                typeof(RarityData),
                typeof(PageSwitcherData)
            };
        }

        protected override Type[] ActionTypesToCreate()
        {
            return new[]
            {
                typeof(SetBasicWeaponDefFromSlotAction),
                typeof(SetRarityFromWeaponDefAction),
                typeof(SetWeaponNameAction),
                typeof(SetWeaponTypeTextAction),
                typeof(SetTierCountAction),
                typeof(SetImageColorRarityAction),
                typeof(SetSlotViewSlotAction),
                typeof(SetWeaponSlotStorageOnClickAction),
                typeof(SetSelectedWeaponFromSlotOnClickAction),
                typeof(OpenPageOnClickAction),
                typeof(SetWeaponSlotTitleOnClickAction),
            };
        }
    }
}
```

### FilteredEntity
FilteredEntity is a special Entity that restricts which data/actions can be
attached based on name prefixes. It also overrides setup to auto-run actions
when active.

Key virtual methods:
- GetAllowedDataNamePrefixes()
  Return allowed ComponentData name prefixes. Return null to allow all.

- GetAllowedActionNamePrefixes()
  Return allowed EntityAction name prefixes. Return null to allow all.

Example from this project:
```csharp
using System;

namespace VladislavTsurikov.EntityDataActionFramework
{
    public class SelectedEntity : FilteredEntity
    {
        protected override Type[] ComponentDataTypesToCreate()
        {
            return new[] { typeof(SelectedData) };
        }

        public override string[] GetAllowedActionNamePrefixes()
        {
            return new[] { "UI/Common/Selected/" };
        }
    }
}
```

## Attributes (detailed)

### [Name("...")]
- Stable identifier for editor/serialization and tooling.
- Keep it unique and consistent.
- Use a path-like naming scheme (e.g., "UI/Gunship/Stat/SetStatValueAction").

### [RequiresData(typeof(A), typeof(B))]
- Ensures these ComponentData types exist on the Entity.
- If a required type is missing, the action will not work.

### [RunOnDirtyData(typeof(A), typeof(B))]
- Action runs when any of these data types become dirty.
- Warning: if the action writes to any of the same data types, it can trigger
  itself again and cause infinite recursion.
- Prefer splitting responsibilities into two actions to avoid loops.

## Extended docs
- Common patterns: Common_Patterns.md
- Best practices: Best_Practices.md
