#if UNITY_EDITOR
using UnityEngine;
using VladislavTsurikov.MegaWorld.Editor.Core.SelectionDatas.Group.TemplatesSystem;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.FilterSettings;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.FilterSettings.MaskFilterSystem;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.FilterSettings.MaskFilterSystem.Noise;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.OverlapCheckSettings;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.OverlapCheckSettings.OverlapChecks;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.ScatterSystem;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.TransformElementSystem;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.PrototypeGameObject;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.PrototypeTerrainObject;

namespace VladislavTsurikov.MegaWorld.Editor.Common.Templates
{
    [Template("Rocks",
        new[] { typeof(AdvancedBrushTool.AdvancedBrushTool), typeof(Runtime.TerrainSpawner.TerrainSpawner) },
        new[] { typeof(PrototypeTerrainObject), typeof(PrototypeGameObject) })]
    public class Rocks : Template
    {
        protected override void Apply(Group group)
        {
            var filterSettings = (FilterSettings)group.GetElement(typeof(FilterSettings));
            var scatterComponentSettings = (ScatterComponentSettings)group.GetElement(typeof(ScatterComponentSettings));

            #region Scatter Settings

            scatterComponentSettings.ScatterStack.Clear();

            var randomGrid = (RandomGrid)scatterComponentSettings.ScatterStack.CreateIfMissingType(typeof(RandomGrid));
            randomGrid.RandomisationType = RandomisationType.Square;
            randomGrid.Vastness = 1;
            randomGrid.GridStep = new Vector2(2.7f, 2.7f);
            randomGrid.FailureRate = 90;

            #endregion

            #region Mask Filters

            filterSettings.FilterType = FilterType.MaskFilter;
            filterSettings.MaskFilterComponentSettings.MaskFilterStack.Clear();

            var noiseFilter =
                (NoiseFilter)filterSettings.MaskFilterComponentSettings.MaskFilterStack.CreateComponent(
                    typeof(NoiseFilter));
            noiseFilter.NoiseSettings = new NoiseSettings
            {
                TransformSettings = new NoiseSettings.NoiseTransformSettings { Scale = new Vector3(37, 40, 37) }
            };

            var remapFilter =
                (MaskOperationsFilter)filterSettings.MaskFilterComponentSettings.MaskFilterStack.CreateComponent(
                    typeof(MaskOperationsFilter));
            remapFilter.MaskOperations = MaskOperations.Remap;
            remapFilter.RemapRange.x = 0.44f;
            remapFilter.RemapRange.y = 0.47f;

            #endregion
        }

        protected override void Apply(Prototype proto)
        {
            var transformComponentSettings =
                (TransformComponentSettings)proto.GetElement(typeof(TransformComponentSettings));
            var overlapCheckSettings = (OverlapCheckSettings)proto.GetElement(typeof(OverlapCheckSettings));

            #region Transform Components

            transformComponentSettings.TransformComponentStack.Clear();

            var rotation =
                (Rotation)transformComponentSettings.TransformComponentStack.CreateIfMissingType(typeof(Rotation));
            rotation.RandomizeOrientationX = 100;
            rotation.RandomizeOrientationY = 100;
            rotation.RandomizeOrientationZ = 100;

            var scale = (Scale)transformComponentSettings.TransformComponentStack.CreateIfMissingType(typeof(Scale));
            scale.MinScale = new Vector3(0.8f, 0.8f, 0.8f);
            scale.MaxScale = new Vector3(1.2f, 1.2f, 1.2f);

            #endregion

            #region OverlapCheckSettings

            overlapCheckSettings.OverlapShapeEnum = OverlapShapeEnum.OBB;
            overlapCheckSettings.ObbCheck.BoundsType = BoundsCheckType.BoundsPrefab;
            overlapCheckSettings.ObbCheck.MultiplyBoundsSize = 1;

            #endregion
        }
    }
}
#endif
