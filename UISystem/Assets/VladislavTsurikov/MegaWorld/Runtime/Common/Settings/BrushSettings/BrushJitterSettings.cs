using System;
using OdinSerializer;
using UnityEngine;
using VladislavTsurikov.ColliderSystem.Runtime;
using VladislavTsurikov.MegaWorld.Runtime.Common.Area;
using VladislavTsurikov.MegaWorld.Runtime.Core.GlobalSettings.ElementsSystem;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group;
using VladislavTsurikov.MegaWorld.Runtime.Core.Utility;
using Random = System.Random;

namespace VladislavTsurikov.MegaWorld.Runtime.Common.Settings.BrushSettings
{
    [Serializable]
    public class BrushJitterSettings
    {
        [OdinSerialize]
        private float _brushRotationJitter;

        [OdinSerialize]
        private float _brushScatter;

        [OdinSerialize]
        private float _brushScatterJitter;

        [OdinSerialize]
        private float _brushSizeJitter;

        public float BrushSizeJitter
        {
            get => _brushSizeJitter;
            set => _brushSizeJitter = Mathf.Clamp01(value);
        }


        public float BrushScatter
        {
            get => _brushScatter;
            set => _brushScatter = Mathf.Clamp01(value);
        }


        public float BrushRotationJitter
        {
            get => _brushRotationJitter;
            set => _brushRotationJitter = Mathf.Clamp01(value);
        }

        public float BrushScatterJitter
        {
            get => _brushScatterJitter;
            set => _brushScatterJitter = Mathf.Clamp01(value);
        }

        public BoxArea GetAreaVariables(BrushSettings brush, Vector3 point, Group group)
        {
            var rand = new Random(Time.frameCount);

            var size = brush.BrushSize;
            size -= brush.BrushRadius * BrushSizeJitter * (float)rand.NextDouble() * 2;

            float rotation = 0;
            rotation += Mathf.Sign((float)rand.NextDouble() - 0.5f) * brush.BrushRotation * BrushRotationJitter *
                        (float)rand.NextDouble();

            Vector3 scatterDir = new Vector3((float)(rand.NextDouble() * 2 - 1), 0, (float)(rand.NextDouble() * 2 - 1))
                .normalized;
            var scatterLengthMultiplier = BrushScatter - (float)rand.NextDouble() * BrushScatterJitter;
            var scatterLength = brush.BrushRadius * scatterLengthMultiplier;

            point += scatterDir * scatterLength;

            LayerSettings layerSettings = GlobalCommonComponentSingleton<LayerSettings>.Instance;

            RayHit rayHit = RaycastUtility.Raycast(RayUtility.GetRayDown(point),
                layerSettings.GetCurrentPaintLayers(group.PrototypeType));

            if (rayHit != null)
            {
                var area = new BoxArea(rayHit, size) { Rotation = rotation, Mask = brush.GetCurrentRaw() };

                return area;
            }

            return null;
        }
    }
}
