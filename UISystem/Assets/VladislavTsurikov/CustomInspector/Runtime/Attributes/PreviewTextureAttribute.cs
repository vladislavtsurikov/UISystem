using System;

namespace VladislavTsurikov.CustomInspector.Runtime
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class PreviewTextureAttribute : Attribute
    {
        public PreviewTextureAttribute(float height = 64f, float width = 0f)
        {
            Height = height;
            Width = width;
        }

        public float Height { get; }
        public float Width { get; }
    }
}
