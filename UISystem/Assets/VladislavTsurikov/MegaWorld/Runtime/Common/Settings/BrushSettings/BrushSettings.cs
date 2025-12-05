using OdinSerializer;
using UnityEngine;
using VladislavTsurikov.ColliderSystem.Runtime;
using VladislavTsurikov.MegaWorld.Runtime.Common.Area;
using VladislavTsurikov.ReflectionUtility;
using Component = VladislavTsurikov.ComponentStack.Runtime.Core.Component;

namespace VladislavTsurikov.MegaWorld.Runtime.Common.Settings.BrushSettings
{
    public enum MaskType
    {
        Custom,
        Procedural
    }

    public enum SpacingEqualsType
    {
        BrushSize,
        HalfBrushSize,
        Custom
    }


    [Name("Brush Settings")]
    public class BrushSettings : Component
    {
        [OdinSerialize]
        private float _brushRotation;

        [OdinSerialize]
        private float _brushSize = 100;

        [OdinSerialize]
        private float _customCustomSpacing = 30;

        public BrushJitterSettings BrushJitterSettings = new();
        public CustomMasks CustomMasks = new();
        public MaskType MaskType = MaskType.Procedural;

        public ProceduralMask ProceduralMask = new();
        public SpacingEqualsType SpacingEqualsType = SpacingEqualsType.HalfBrushSize;

        public float CustomSpacing
        {
            set => _customCustomSpacing = Mathf.Max(0.01f, value);
            get => _customCustomSpacing;
        }

        public float Spacing
        {
            get
            {
                switch (SpacingEqualsType)
                {
                    case SpacingEqualsType.BrushSize:
                    {
                        return BrushSize;
                    }
                    case SpacingEqualsType.HalfBrushSize:
                    {
                        return BrushSize / 2;
                    }
                    default:
                    {
                        return Mathf.Max(0.01f, CustomSpacing);
                    }
                }
            }
        }

        public float BrushRotation
        {
            get => _brushRotation;
            set => _brushRotation = value;
        }

        public float BrushSize
        {
            get => _brushSize;
            set => _brushSize = Mathf.Max(0.01f, value);
        }

        public float BrushRadius => _brushSize / 2;

        public Texture2D GetCurrentRaw()
        {
            switch (MaskType)
            {
                case MaskType.Custom:
                {
                    Texture2D texture = CustomMasks.GetSelectedBrush();

                    return texture;
                }
                case MaskType.Procedural:
                {
                    Texture2D texture = ProceduralMask.Mask;

                    return texture;
                }
            }

            return Texture2D.whiteTexture;
        }

        public void ScrollBrushRadiusEvent()
        {
            if (Event.current.shift)
            {
                if (Event.current.type == EventType.ScrollWheel)
                {
                    BrushSize += Event.current.delta.y;
                    Event.current.Use();
                }
            }
        }

        public BoxArea GetAreaVariables(RayHit hit)
        {
            if (hit == null)
            {
                return null;
            }

            var area = new BoxArea(hit, BrushSize) { Mask = GetCurrentRaw(), Rotation = BrushRotation };

            return area;
        }
    }
}
