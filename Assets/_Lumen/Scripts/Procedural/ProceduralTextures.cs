using UnityEngine;

namespace Lumen
{
    /// <summary>CG CONCEPT 6 - textures generated in code (no image assets).</summary>
    public static class ProceduralTextures
    {
        /// <summary>Neon grid: dark cells with bright lines at the borders, softly anti-aliased by distance to the edge.</summary>
        public static Texture2D NeonGrid(int size = 256, int cells = 4)
        {
            var tex = new Texture2D(size, size, TextureFormat.RGBA32, true) { name = "NeonGrid", wrapMode = TextureWrapMode.Repeat, filterMode = FilterMode.Trilinear, anisoLevel = 8 };
            var px = new Color32[size * size];
            float cell = size / (float)cells;
            for (int y = 0; y < size; y++)
                for (int x = 0; x < size; x++)
                {
                    float fx = (x % cell) / cell, fy = (y % cell) / cell;
                    float d = Mathf.Min(Mathf.Min(fx, 1 - fx), Mathf.Min(fy, 1 - fy)) * cell;   // pixels to nearest line
                    float line = Mathf.Clamp01(1.6f - d * 0.8f);
                    line *= line;
                    float v = 0.10f + 0.9f * line;
                    px[y * size + x] = new Color32((byte)(v * 255), (byte)(v * 255), (byte)(v * 255), 255);
                }
            tex.SetPixels32(px);
            tex.Apply(true);
            return tex;
        }

        /// <summary>Soft round sprite used by all particle systems.</summary>
        public static Texture2D SoftDot(int size = 64)
        {
            var tex = new Texture2D(size, size, TextureFormat.RGBA32, false) { name = "SoftDot", wrapMode = TextureWrapMode.Clamp };
            var px = new Color32[size * size];
            for (int y = 0; y < size; y++)
                for (int x = 0; x < size; x++)
                {
                    float dx = (x + 0.5f) / size * 2 - 1, dy = (y + 0.5f) / size * 2 - 1;
                    float a = Mathf.Clamp01(1f - Mathf.Sqrt(dx * dx + dy * dy));
                    a *= a;
                    px[y * size + x] = new Color32(255, 255, 255, (byte)(a * 255));
                }
            tex.SetPixels32(px); tex.Apply();
            return tex;
        }

        /// <summary>Star flare sprite with cross diffraction spikes and soft glowing nucleus.</summary>
        public static Texture2D StarFlare(int size = 64)
        {
            var tex = new Texture2D(size, size, TextureFormat.RGBA32, false) { name = "StarFlare", wrapMode = TextureWrapMode.Clamp };
            var px = new Color32[size * size];
            float half = (size - 1) * 0.5f;
            for (int y = 0; y < size; y++)
            {
                float dy = (y - half) / half;
                float absY = Mathf.Abs(dy);
                for (int x = 0; x < size; x++)
                {
                    float dx = (x - half) / half;
                    float absX = Mathf.Abs(dx);
                    float r = Mathf.Sqrt(dx * dx + dy * dy);

                    // Soft central glow core
                    float core = Mathf.Clamp01(1f - r * 1.5f);
                    core = core * core * core;

                    // 4-point diffraction spike flare
                    float spikeH = Mathf.Clamp01(1f - absX * 1.1f) * Mathf.Clamp01(1f - absY * 7f);
                    float spikeV = Mathf.Clamp01(1f - absY * 1.1f) * Mathf.Clamp01(1f - absX * 7f);
                    float spike = (spikeH + spikeV) * 0.8f;

                    float alpha = Mathf.Clamp01(core + spike);
                    byte a = (byte)(alpha * 255f);
                    px[y * size + x] = new Color32(255, 255, 255, a);
                }
            }
            tex.SetPixels32(px);
            tex.Apply();
            return tex;
        }
    }
}
