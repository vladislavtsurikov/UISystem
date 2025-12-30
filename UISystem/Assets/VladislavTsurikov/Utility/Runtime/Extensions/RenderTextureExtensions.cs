using UnityEngine;

namespace VladislavTsurikov.Utility.Runtime.Extensions
{
    public static class RenderTextureExtensions
    {
        public static Vector2Int ComputeRenderTextureSize(this Vector2 areaSize, int textureSize)
        {
            float widthWorld = areaSize.x;
            float heightWorld = Mathf.Abs(areaSize.y) < 0.001f ? widthWorld : areaSize.y;
            float aspect = widthWorld / heightWorld;

            int width = textureSize;
            int height = Mathf.RoundToInt(textureSize / aspect);

            return new Vector2Int(width, height);
        }

        public static RenderTexture RecreateTemporary(this RenderTexture renderTexture, Vector2Int size, RenderTextureFormat format)
        {
            if (renderTexture != null)
            {
                renderTexture.Release();
            }

            renderTexture = RenderTexture.GetTemporary(size.x, size.y, 0, format);
            return renderTexture;
        }

        public static Texture2D RecreateTexture(this Texture2D texture, Vector2Int size, TextureFormat format)
        {
            bool invalid = texture == null || texture.width != size.x || texture.height != size.y;

            if (!invalid)
            {
                return texture;
            }

            if (texture != null)
            {
                Object.DestroyImmediate(texture);
            }

            texture = new Texture2D(size.x, size.y, format, false);
            return texture;
        }

        public static void CopyToTexture(this RenderTexture renderTexture, Texture2D texture)
        {
            RenderTexture previous = RenderTexture.active;
            RenderTexture.active = renderTexture;

            texture.ReadPixels(new Rect(0, 0, texture.width, texture.height), 0, 0);
            texture.Apply();

            RenderTexture.active = previous;
        }
    }
}
