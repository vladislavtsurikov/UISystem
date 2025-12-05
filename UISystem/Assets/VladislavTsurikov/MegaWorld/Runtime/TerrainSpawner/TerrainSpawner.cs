using System;
using System.Runtime.Serialization;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VladislavTsurikov.ColliderSystem.Runtime;
using VladislavTsurikov.MegaWorld.Runtime.Common.Area;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.FilterSettings;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.FilterSettings.MaskFilterSystem;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.OverlapCheckSettings;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.ScatterSystem;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.TransformElementSystem;
using VladislavTsurikov.MegaWorld.Runtime.Common.Stamper;
using VladislavTsurikov.MegaWorld.Runtime.Core.GlobalSettings.ElementsSystem;
using VladislavTsurikov.MegaWorld.Runtime.Core.MonoBehaviour;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Attributes;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.ElementsSystem;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.PrototypeGameObject;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.PrototypeTerrainDetail;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.PrototypeTerrainObject;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.PrototypeTerrainTexture;
using VladislavTsurikov.MegaWorld.Runtime.Core.Utility;
using VladislavTsurikov.ReflectionUtility;
using Area = VladislavTsurikov.MegaWorld.Runtime.Common.Stamper.Area;
#if UNITY_EDITOR
using VladislavTsurikov.MegaWorld.Editor.Common.Stamper;
#endif

namespace VladislavTsurikov.MegaWorld.Runtime.TerrainSpawner
{
    [ExecuteInEditMode]
    [Name("Terrain Spawner")]
    [SupportMultipleSelectedGroups]
    [SupportedPrototypeTypes(new[]
    {
        typeof(PrototypeTerrainObject), typeof(PrototypeGameObject), typeof(PrototypeTerrainDetail)
    })]
    [AddMonoBehaviourComponents(new[] { typeof(Area), typeof(StamperControllerSettings) })]
    [AddGlobalCommonComponents(new[] { typeof(LayerSettings) })]
    [AddGeneralPrototypeComponents(typeof(PrototypeTerrainObject),
        new[] { typeof(SuccessSettings), typeof(OverlapCheckSettings), typeof(TransformComponentSettings) })]
    [AddGeneralPrototypeComponents(typeof(PrototypeGameObject),
        new[] { typeof(SuccessSettings), typeof(OverlapCheckSettings), typeof(TransformComponentSettings) })]
    [AddGeneralPrototypeComponents(typeof(PrototypeTerrainDetail),
        new[] { typeof(SpawnDetailSettings), typeof(MaskFilterComponentSettings) })]
    [AddGeneralPrototypeComponents(typeof(PrototypeTerrainTexture), new[] { typeof(MaskFilterComponentSettings) })]
    [AddGeneralGroupComponents(new[] { typeof(PrototypeTerrainObject), typeof(PrototypeGameObject) },
        new[] { typeof(RandomSeedSettings), typeof(ScatterComponentSettings), typeof(FilterSettings) })]
    public class TerrainSpawner : StamperTool
    {
        [NonSerialized]
        private Area _area;

        [NonSerialized]
        private StamperControllerSettings _stamperControllerSettings;

        [NonSerialized]
        private TerrainsMaskManager _terrainsMaskManager = new();

#if UNITY_EDITOR
        [NonSerialized]
        public StamperVisualisation StamperVisualisation = new();
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

        [OnDeserializing]
        private void Initialize()
        {
            _terrainsMaskManager = new TerrainsMaskManager();

#if UNITY_EDITOR
            StamperVisualisation = new StamperVisualisation();
#endif
        }

        private protected override void OnStamperEnable()
        {
#if UNITY_EDITOR
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

            for (var typeIndex = 0; typeIndex < Data.GroupList.Count; typeIndex++)
            {
                token.ThrowIfCancellationRequested();
#if UNITY_EDITOR
                UpdateDisplayProgressBar("Running", "Running " + Data.GroupList[typeIndex].name);
#endif

                RayHit rayHit = RaycastUtility.Raycast(RayUtility.GetRayDown(transform.position),
                    GlobalCommonComponentSingleton<LayerSettings>.Instance.GetCurrentPaintLayers(
                        Data.GroupList[typeIndex].PrototypeType));

                if (rayHit == null)
                {
                    continue;
                }

                BoxArea boxArea = Area.GetAreaVariables(rayHit);

                await SpawnGroup(token, Data.GroupList[typeIndex], boxArea, displayProgressBar);

                completedTypes++;
                SpawnProgress = completedTypes / (float)maxTypes;
            }
        }

        private async UniTask SpawnGroup(CancellationToken token, Group group, BoxArea boxArea, bool displayProgressBar)
        {
            if (group.HasAllActivePrototypes())
            {
                if (group.PrototypeType == typeof(PrototypeGameObject))
                {
                    var randomSeedSettings = (RandomSeedSettings)group.GetElement(typeof(RandomSeedSettings));
                    randomSeedSettings.GenerateRandomSeedIfNecessary();

                    await Utility.SpawnGroup.SpawnGameObject(token, group, _terrainsMaskManager, boxArea,
                        displayProgressBar);
                }
#if RENDERER_STACK
                else if (group.PrototypeType == typeof(PrototypeTerrainObject))
                {
                    var randomSeedSettings =
                        (RandomSeedSettings)group.GetElement(typeof(RandomSeedSettings));
                    randomSeedSettings.GenerateRandomSeedIfNecessary();

                    await Utility.SpawnGroup.SpawnTerrainObject(token, group, _terrainsMaskManager, boxArea,
                        displayProgressBar);
                }
#endif
                else if (group.PrototypeType == typeof(PrototypeTerrainDetail))
                {
                    await Utility.SpawnGroup.SpawnTerrainDetails(token, group, group.PrototypeList,
                        _terrainsMaskManager, boxArea);
                }
            }

            _terrainsMaskManager.Dispose();
        }
    }
}
