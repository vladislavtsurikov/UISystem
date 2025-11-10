using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VladislavTsurikov.ColliderSystem.Runtime;
using VladislavTsurikov.MegaWorld.Runtime.Common.Area;
using VladislavTsurikov.MegaWorld.Runtime.Common.PhysXPainter.Settings;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.FilterSettings;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.ScatterSystem;
using VladislavTsurikov.MegaWorld.Runtime.Common.Stamper;
using VladislavTsurikov.MegaWorld.Runtime.Core.GlobalSettings.ElementsSystem;
using VladislavTsurikov.MegaWorld.Runtime.Core.MonoBehaviour;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Attributes;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.ElementsSystem;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.PrototypeGameObject;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.PrototypeTerrainObject;
using VladislavTsurikov.MegaWorld.Runtime.Core.Utility;
using VladislavTsurikov.ReflectionUtility;
using Area = VladislavTsurikov.MegaWorld.Runtime.Common.Stamper.Area;
#if UNITY_EDITOR
using VladislavTsurikov.MegaWorld.Editor.GravitySpawner;
#endif

namespace VladislavTsurikov.MegaWorld.Runtime.GravitySpawner
{
    [ExecuteInEditMode]
    [Name("Gravity Spawner")]
    [SupportMultipleSelectedGroups]
    [AddMonoBehaviourComponents(new[] { typeof(Area), typeof(StamperControllerSettings), typeof(PhysicsEffects) })]
    [SupportedPrototypeTypes(new[] { typeof(PrototypeTerrainObject), typeof(PrototypeGameObject) })]
    [AddGlobalCommonComponents(new[] { typeof(LayerSettings) })]
    [AddGeneralPrototypeComponents(new[] { typeof(PrototypeGameObject), typeof(PrototypeTerrainObject) },
        new[] { typeof(SuccessSettings), typeof(PhysicsTransformComponentSettings) })]
    [AddGeneralGroupComponents(new[] { typeof(PrototypeGameObject), typeof(PrototypeTerrainObject) },
        new[] { typeof(RandomSeedSettings), typeof(ScatterComponentSettings) })]
    [AddGroupComponents(new[] { typeof(PrototypeGameObject), typeof(PrototypeTerrainObject) },
        new[] { typeof(FilterSettings) })]
    public class GravitySpawner : StamperTool
    {
        [NonSerialized]
        private Area _area;

        [NonSerialized]
        private PhysicsEffects _physicsEffects;

        [NonSerialized]
        private StamperControllerSettings _stamperControllerSettings;

        [NonSerialized]
        private TerrainsMaskManager _terrainsMaskManager = new();

#if UNITY_EDITOR
        [NonSerialized]
        public GravitySpawnerVisualisation StamperVisualisation = new();
#endif

        public Area Area
        {
            get
            {
                if (_area == null || _area.IsHappenedReset)
                {
                    _area = (Area)GetElement(typeof(Area));
                }

                return _area;
            }
        }

        public StamperControllerSettings StamperControllerSettings
        {
            get
            {
                if (_stamperControllerSettings == null || _stamperControllerSettings.IsHappenedReset)
                {
                    _stamperControllerSettings =
                        (StamperControllerSettings)GetElement(typeof(StamperControllerSettings));
                }

                return _stamperControllerSettings;
            }
        }

        public PhysicsEffects PhysicsEffects
        {
            get
            {
                if (_physicsEffects == null || _physicsEffects.IsHappenedReset)
                {
                    _physicsEffects = (PhysicsEffects)GetElement(typeof(PhysicsEffects));
                }

                return _physicsEffects;
            }
        }

        private protected override void OnStamperEnable()
        {
            _terrainsMaskManager = new TerrainsMaskManager();

#if UNITY_EDITOR
            StamperVisualisation = new GravitySpawnerVisualisation();

            void Action()
            {
                StamperVisualisation.StamperMaskFilterVisualisation.NeedUpdateMask = true;
            }

            Area.OnSetAreaBounds -= Action;
            Area.OnSetAreaBounds += Action;
#endif

            Area.SetAreaBoundsIfNecessary(this, true);
        }

        protected override void OnUpdate() => Area.SetAreaBoundsIfNecessary(this);

        protected override async UniTask Spawn(CancellationToken token, bool displayProgressBar)
        {
            var maxTypes = Data.GroupList.Count;
            var completedTypes = 0;

            foreach (Group group in Data.GroupList)
            {
                token.ThrowIfCancellationRequested();

#if UNITY_EDITOR
                UpdateDisplayProgressBar("Running", "Running " + group.name);
#endif

                RayHit rayHit = RaycastUtility.Raycast(RayUtility.GetRayDown(transform.position),
                    GlobalCommonComponentSingleton<LayerSettings>.Instance.GetCurrentPaintLayers(group.PrototypeType));

                if (rayHit == null)
                {
                    continue;
                }

                BoxArea boxArea = Area.GetAreaVariables(rayHit);

                await SpawnGroup(token, group, boxArea);

                completedTypes++;
                SpawnProgress = completedTypes / (float)maxTypes;
            }
        }

        public override void OnCancelSpawn() => DestroyGameObjectsForPrototypeTerrainObject();

        public void DestroyGameObjectsForPrototypeTerrainObject() =>
            ContainerForGameObjectsUtility.DestroyGameObjects<PrototypeTerrainObject>(Data);

        private async UniTask SpawnGroup(CancellationToken token, Group group, BoxArea boxArea)
        {
            if (group.HasAllActivePrototypes())
            {
                if (group.PrototypeType == typeof(PrototypeGameObject))
                {
                    var randomSeedSettings = (RandomSeedSettings)group.GetElement(typeof(RandomSeedSettings));
                    randomSeedSettings.GenerateRandomSeedIfNecessary();

                    await Utility.SpawnGroup.SpawnGameObject(token, this, group, _terrainsMaskManager, boxArea);
                }
#if RENDERER_STACK
                else if (group.PrototypeType == typeof(PrototypeTerrainObject))
                {
                    var randomSeedSettings =
                        (RandomSeedSettings)group.GetElement(typeof(RandomSeedSettings));
                    randomSeedSettings.GenerateRandomSeedIfNecessary();

                    await Utility.SpawnGroup.SpawnTerrainObject(token, this, group, _terrainsMaskManager, boxArea);
                }
#endif
            }

            _terrainsMaskManager.Dispose();

            await UniTask.Yield();
        }
    }
}
