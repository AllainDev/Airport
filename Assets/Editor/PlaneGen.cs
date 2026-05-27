using UnityEngine;
using UnityEditor;
public class PlaneGen
{
    [MenuItem("Tools/Gen 100x100")]
    public static void Gen()
    {
        GameObject g = new GameObject("Runway");
        MeshFilter mf = g.AddComponent<MeshFilter>();
        MeshRenderer mr = g.AddComponent<MeshRenderer>();
        Mesh m = new Mesh();
        int r = 100;
        float s = 500f;
        Vector3[] v = new Vector3[(r + 1) * (r + 1)];
        Vector2[] u = new Vector2[v.Length];
        int[] t = new int[r * r * 6];
        for (int i = 0, y = 0; y <= r; y++)
        {
            for (int x = 0; x <= r; x++, i++)
            {
                v[i] = new Vector3(((float)x / r - 0.5f) * s, 0, ((float)y / r - 0.5f) * s);
                u[i] = new Vector2((float)x / r, (float)y / r);
            }
        }
        for (int ti = 0, vi = 0, y = 0; y < r; y++, vi++)
        {
            for (int x = 0; x < r; x++, ti += 6, vi++)
            {
                t[ti] = vi; t[ti + 1] = vi + r + 1; t[ti + 2] = vi + 1;
                t[ti + 3] = vi + 1; t[ti + 4] = vi + r + 1; t[ti + 5] = vi + r + 2;
            }
        }
        m.vertices = v; m.uv = u; m.triangles = t; m.RecalculateNormals();
        mf.mesh = m;
        AssetDatabase.CreateAsset(m, "Assets/Runway100x100.asset");
        AssetDatabase.SaveAssets();
    }
}