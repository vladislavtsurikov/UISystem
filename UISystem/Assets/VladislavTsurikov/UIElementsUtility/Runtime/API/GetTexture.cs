//.........................
//.....Generated File......
//.........................
//.......Do not edit.......
//.........................

using UnityEngine;
using VladislavTsurikov.UIElementsUtility.Runtime.Core.Utility;
using VladislavTsurikov.UIElementsUtility.Runtime.Groups.Textures;

namespace VladislavTsurikov.UIElementsUtility.Runtime
{
    public static class GetTexture
    {
        public static class Images
        {
            public enum TextureName
            {
                Minus,
                Plus
            }

            private static TextureGroup s_textureGroup;
            private static Texture2D s_minus;
            private static Texture2D s_plus;

            private static TextureGroup TextureGroup => s_textureGroup != null
                ? s_textureGroup
                : s_textureGroup = DataGroupUtility.GetGroup<TextureGroup, TextureInfo>("Images");

            public static Texture2D Minus => s_minus ? s_minus : s_minus = GetTexture2D(TextureName.Minus);

            public static Texture2D Plus => s_plus ? s_plus : s_plus = GetTexture2D(TextureName.Plus);

            private static Texture2D GetTexture2D(TextureName textureName) =>
                TextureGroup.GetTexture(textureName.ToString());
        }
    }
}
