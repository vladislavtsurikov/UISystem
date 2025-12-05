using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VladislavTsurikov.ColliderSystem.Runtime;
using VladislavTsurikov.MegaWorld.Runtime.Common.Area;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings;
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
using VladislavTsurikov.MegaWorld.Runtime.Core.Utility;
using VladislavTsurikov.ReflectionUtility;
using Common_Stamper_Area = VladislavTsurikov.MegaWorld.Runtime.Common.Stamper.Area;

namespace VladislavTsurikov.MegaWorld.Runtime.Spawner
{
    [ExecuteInEditMode]
    [Name("Spawner")]
    [SupportMultipleSelectedGroups]
    [SupportedPrototypeTypes(new[] { typeof(PrototypeGameObject) })]
    [AddMonoBehaviourComponents(new[] { typeof(Common_Stamper_Area), typeof(StamperControllerSettings) })]
    [AddGlobalCommonComponents(new[] { typeof(LayerSettings) })]
    [AddGeneralPrototypeComponents(typeof(PrototypeGameObject),
        new[] { typeof(SuccessSettings), typeof(OverlapCheckSettings), typeof(TransformComponentSettings) })]
    [AddGeneralGroupComponents(new[] { typeof(PrototypeGameObject) },
        new[] { typeof(RandomSeedSettings), typeof(ScatterComponentSettings) })]
    public class Spawner : StamperTool
    {
        [NonSerialized]
        private Common_Stamper_Area _area;

        [NonSerialized]
        private StamperControllerSettings _stamperControllerSettings;

        public Common_Stamper_Area Area
        {
            get
            {
                if (_area == null || _area.IsHappenedReset)
                {
                    _area = (Common_Stamper_Area)GetElement(typeof(Common_Stamper_Area));
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

        private protected override void OnStamperEnable() => Area.SetAreaBoundsIfNecessary(this, true);

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

                    await Utility.SpawnGroup.SpawnGameObject(token, group, boxArea, displayProgressBar);
                }
            }
        }
    }
}
