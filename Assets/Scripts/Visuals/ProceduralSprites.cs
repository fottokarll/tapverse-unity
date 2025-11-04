using UnityEngine;

namespace TapVerse.Visuals
{
    public static class ProceduralSprites
    {
        private static Texture2D _universeCoreTexture;
        private static Sprite _universeCoreSprite;
        private static Texture2D _sparkTexture;
        private static Sprite _sparkSprite;

        public static Texture2D GetUniverseCoreTexture()
        {
            return _universeCoreTexture ??= CreateUniverseCoreTexture();
        }

        public static Sprite GetUniverseCoreSprite()
        {
            return _universeCoreSprite ??= CreateSprite(GetUniverseCoreTexture());
        }

        public static Texture2D GetSparkTexture()
        {
            return _sparkTexture ??= CreateSparkTexture();
        }

        public static Sprite GetSparkSprite()
        {
            return _sparkSprite ??= CreateSprite(GetSparkTexture());
        }

        public static Texture2D CreateUniverseCoreTexture(int size = 512)
        {
            var texture = new Texture2D(size, size, TextureFormat.RGBA32, false)
            {
                wrapMode = TextureWrapMode.Clamp,
                filterMode = FilterMode.Bilinear
            };

            var center = (size - 1) * 0.5f;
            for (var y = 0; y < size; y++)
            {
                for (var x = 0; x < size; x++)
                {
                    var dx = x - center;
                    var dy = y - center;
                    var distance = Mathf.Sqrt(dx * dx + dy * dy);
                    var normalized = Mathf.Clamp01(distance / center);
                    var glow = 1f - normalized;
                    var falloff = Mathf.Pow(glow, 1.5f);
                    var color = new Color(
                        Mathf.Lerp(0.08f, 0.95f, falloff),
                        Mathf.Lerp(0.05f, 0.85f, falloff),
                        Mathf.Lerp(0.12f, 1f, falloff),
                        Mathf.Lerp(0.05f, 1f, falloff));
                    texture.SetPixel(x, y, color);
                }
            }

            texture.Apply();
            return texture;
        }

        public static Texture2D CreateSparkTexture(int size = 64)
        {
            var texture = new Texture2D(size, size, TextureFormat.RGBA32, false)
            {
                wrapMode = TextureWrapMode.Clamp,
                filterMode = FilterMode.Bilinear
            };

            var center = (size - 1) * 0.5f;
            var radius = center;
            for (var y = 0; y < size; y++)
            {
                for (var x = 0; x < size; x++)
                {
                    var dx = x - center;
                    var dy = y - center;
                    var distance = Mathf.Sqrt(dx * dx + dy * dy);
                    var normalized = Mathf.Clamp01(distance / radius);
                    var edge = 1f - normalized;
                    var falloff = edge * edge;
                    var alpha = Mathf.Clamp01(falloff);
                    var color = new Color(1f, 1f, 1f, alpha);
                    texture.SetPixel(x, y, color);
                }
            }

            texture.Apply();
            return texture;
        }

        private static Sprite CreateSprite(Texture2D texture)
        {
            return Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100f);
        }
    }
}
