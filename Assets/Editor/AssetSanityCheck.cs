using UnityEditor;
using UnityEngine;
using System.IO;
using System.Linq;

public class AssetSanityCheck : EditorWindow
{
    [MenuItem("Tools/Generate Sanity Report")]
    public static void GenerateReport()
    {
        string path = "Asset_Sanity_Report.txt";
        using StreamWriter writer = new StreamWriter(path);

        var allObjects = FindObjectsByType<GameObject>(FindObjectsSortMode.None);
        bool noDynamic = allObjects.All(o => o.isStatic || o.GetComponent<Camera>() != null || o.GetComponent<Light>() != null || o.GetComponent<WindZone>() != null);
        writer.WriteLine($"Scene phang sach khong chua drone/thuc the dong: {noDynamic}");

        bool polyLimitOk = true;
        foreach (var mf in FindObjectsByType<MeshFilter>(FindObjectsSortMode.None))
        {
            if (mf.sharedMesh != null && (mf.sharedMesh.triangles.Length / 3) > 50000)
            {
                polyLimitOk = false;
                break;
            }
        }
        writer.WriteLine($"Gioi han da giac mo hinh duoi 50.000: {polyLimitOk}");

        bool noTransparent = true;
        foreach (var mr in FindObjectsByType<MeshRenderer>(FindObjectsSortMode.None))
        {
            if (mr.sharedMaterials.Any(m => m != null && m.renderQueue >= 3000))
            {
                noTransparent = false;
                break;
            }
        }
        writer.WriteLine($"Khong chua vat lieu Transparent sai quy dinh: {noTransparent}");

        bool isLinear = PlayerSettings.colorSpace == ColorSpace.Linear;
        writer.WriteLine($"Project Settings khoa cung o Linear Color Space: {isLinear}");

        int batches = UnityEditor.UnityStats.batches;
        writer.WriteLine($"Tong so lenh goi ve (Batches) duoi 500: {batches < 500} (Hien tai: {batches})");

        Debug.Log($"Đã trích xuất báo cáo tại: {path}");
    }
}