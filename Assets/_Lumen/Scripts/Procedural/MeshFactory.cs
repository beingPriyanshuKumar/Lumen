using UnityEngine;

namespace Lumen
{
    /// <summary>Hand-built meshes (vertex + index buffers written by code).</summary>
    public static class MeshFactory
    {
        /// <summary>Elongated octahedron (crystal) with flat per-face normals: 8 faces x 3 unique vertices.</summary>
        public static Mesh Crystal(float radius, float halfHeight)
        {
            Vector3 top = new Vector3(0, halfHeight, 0), bot = new Vector3(0, -halfHeight, 0);
            Vector3[] eq = { new Vector3(radius, 0, 0), new Vector3(0, 0, radius), new Vector3(-radius, 0, 0), new Vector3(0, 0, -radius) };
            var v = new Vector3[24]; var n = new Vector3[24]; var tri = new int[24]; var uv = new Vector2[24]; var col = new Color[24];
            int k = 0;
            for (int i = 0; i < 4; i++)
            {
                Vector3 a = eq[i], b = eq[(i + 1) % 4];
                AddFace(v, n, uv, col, tri, ref k, top, b, a);    // upper face (CCW seen from outside)
                AddFace(v, n, uv, col, tri, ref k, bot, a, b);    // lower face
            }
            var m = new Mesh { name = "Crystal", vertices = v, normals = n, uv = uv, colors = col, triangles = tri };
            m.RecalculateBounds();
            return m;
        }

        static void AddFace(Vector3[] v, Vector3[] n, Vector2[] uv, Color[] col, int[] tri, ref int k, Vector3 a, Vector3 b, Vector3 c)
        {
            Vector3 fn = Vector3.Cross(b - a, c - a).normalized;
            Vector3[] p = { a, b, c };
            Vector2[] t = { new Vector2(0.5f, 1), new Vector2(0, 0), new Vector2(1, 0) };
            for (int i = 0; i < 3; i++) { v[k] = p[i]; n[k] = fn; uv[k] = t[i]; col[k] = Color.white; tri[k] = k; k++; }
        }

        /// <summary>Flat disc in the XY plane (normal -Z/+Z), uv in [0,1]. rings x segments polar grid.</summary>
        public static Mesh Disc(float radius, int rings = 24, int segments = 64)
        {
            var v = new Vector3[1 + rings * segments]; var uv = new Vector2[v.Length]; var n = new Vector3[v.Length];
            v[0] = Vector3.zero; uv[0] = new Vector2(0.5f, 0.5f); n[0] = Vector3.back;
            for (int r = 1; r <= rings; r++)
                for (int s = 0; s < segments; s++)
                {
                    float rad = radius * r / rings, a = s * Mathf.PI * 2f / segments;
                    int i = 1 + (r - 1) * segments + s;
                    v[i] = new Vector3(Mathf.Cos(a) * rad, Mathf.Sin(a) * rad, 0);
                    uv[i] = new Vector2(0.5f + 0.5f * r / rings * Mathf.Cos(a), 0.5f + 0.5f * r / rings * Mathf.Sin(a));
                    n[i] = Vector3.back;
                }
            var tri = new System.Collections.Generic.List<int>();
            for (int s = 0; s < segments; s++) { tri.Add(0); tri.Add(1 + s); tri.Add(1 + (s + 1) % segments); }
            for (int r = 1; r < rings; r++)
                for (int s = 0; s < segments; s++)
                {
                    int a = 1 + (r - 1) * segments + s, b = 1 + (r - 1) * segments + (s + 1) % segments;
                    int c = 1 + r * segments + s, d = 1 + r * segments + (s + 1) % segments;
                    tri.Add(a); tri.Add(c); tri.Add(b); tri.Add(b); tri.Add(c); tri.Add(d);
                }
            var m = new Mesh { name = "Disc", vertices = v, uv = uv, normals = n, triangles = tri.ToArray() };
            m.RecalculateBounds();
            return m;
        }

        /// <summary>Quad covering [-0.5,0.5]; used as a full-screen overlay (vertex shader writes clip space).</summary>
        public static Mesh FullscreenQuad()
        {
            var m = new Mesh { name = "FullscreenQuad" };
            m.vertices = new[] { new Vector3(-0.5f, -0.5f, 0), new Vector3(0.5f, -0.5f, 0), new Vector3(-0.5f, 0.5f, 0), new Vector3(0.5f, 0.5f, 0) };
            m.triangles = new[] { 0, 2, 1, 2, 3, 1 };
            m.bounds = new Bounds(Vector3.zero, Vector3.one * 100000f);   // never frustum-culled
            return m;
        }
    }
}
