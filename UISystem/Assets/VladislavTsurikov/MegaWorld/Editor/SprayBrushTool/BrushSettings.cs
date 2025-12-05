using OdinSerializer;
using UnityEngine;
using VladislavTsurikov.ColliderSystem.Runtime;
using VladislavTsurikov.MegaWorld.Runtime.Common.Area;
using VladislavTsurikov.ReflectionUtility;
using Component = VladislavTsurikov.ComponentStack.Runtime.Core.Component;

namespace VladislavTsurikov.MegaWorld.Editor.SprayBrushTool
{
    [Name("Brush Settings")]
    public class BrushSettings : Component
    {
        [OdinSerialize]
        private float _brushSize = 100;

        [OdinSerialize]
        private float _brushSpacing = 30;

        public float Spacing
        {
            set => _brushSpacing = Mathf.Max(0.01f, value);
            get => _brushSpacing;
        }

        public float BrushSize
        {
            get => _brushSize;
            set => _brushSize = Mathf.Max(0.01f, value);
        }

        public float BrushRadius => _brushSize / 2;

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
            var area = new BoxArea(hit, BrushSize) { Mask = Texture2D.whiteTexture };

            return area;
        }
    }
}
