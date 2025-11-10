using UnityEngine;
using VladislavTsurikov.UnityUtility.Runtime;
using VladislavTsurikov.Utility.Runtime;

namespace VladislavTsurikov.MegaWorld.Runtime.Common.Area
{
    public class Area
    {
        private Vector3 _size;

        public Texture2D Mask = Texture2D.whiteTexture;

        public float Rotation = 0;

        protected Area(Vector3 center, Vector3 size)
        {
            Center = center;
            _size = size;
        }

        protected Area(Vector3 center, float size) : this(center, new Vector3(size, size, size))
        {
        }

        public float SizeMultiplier
        {
            get
            {
                if (Rotation == 0)
                {
                    return 1;
                }

                var sizeMultiplier = Mathf.Abs(CosAngle);
                sizeMultiplier += Mathf.Abs(SinAngle);

                return sizeMultiplier;
            }
        }

        public Vector3 Size
        {
            get => _size * SizeMultiplier;
            protected set => _size = value;
        }

        public float CosAngle => Mathf.Cos(Rotation * Mathf.Deg2Rad);
        public float SinAngle => Mathf.Sin(Rotation * Mathf.Deg2Rad);

        public Vector3 Center { get; }

        public Bounds Bounds =>
            new() { size = new Vector3(Size.x, Size.y, Size.z), center = Center };

        public bool Contains(Vector3 point) => Bounds.Contains(point);

        public bool Contains(Vector2 point) => RectExtension.CreateRectFromBounds(Bounds).Contains(point);

        public float GetAlpha(Vector2 pos, Vector2 size)
        {
            if (Mask == null)
            {
                return 1.0f;
            }

            pos += Vector2Int.one;

            if (Rotation == 0.0f)
            {
                return TextureUtility.GetAlpha(pos, size, Mask);
            }

            Vector2 halfTarget = size / 2.0f;
            Vector2 origin = pos - halfTarget;
            origin *= SizeMultiplier;
            origin = new Vector2(
                origin.x * CosAngle - origin.y * SinAngle + halfTarget.x,
                origin.x * SinAngle + origin.y * CosAngle + halfTarget.y);

            if (origin.x < 0.0f || origin.x > size.x || origin.y < 0.0f || origin.y > size.y)
            {
                return 0.0f;
            }

            return TextureUtility.GetAlpha(origin, size, Mask);
        }
    }
}
