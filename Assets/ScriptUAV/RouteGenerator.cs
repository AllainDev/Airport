using UnityEngine;

public class RouteGenerator : MonoBehaviour
{
    public UAV_Patrol patrolScript;

    [ContextMenu("Build Route")]
    public void BuildRoute()
    {
        Vector3[] route = {
            // 1. Xuất phát từ màn nhiệt
            new Vector3(-45f, 40f, 260f),   
            
            // 2. Hạ gầm cực thấp (Y=12), lướt dọc theo CHÍNH GIỮA hàng cây (X=-45)
            new Vector3(-45f, 12f, 220f),
            new Vector3(-45f, 12f, 0f),
            new Vector3(-45f, 12f, -100f),  
            
            // 3. Vòng nhẹ sang trái, lướt ngay trên nóc dàn máy bay đỗ (X=-90)
            new Vector3(-90f, 25f, -150f),  
            
            // 4. Tiếp tục bẻ trái, quét toàn bộ bãi đỗ xe ô tô và nhà ga (X=-186, Z=-250)
            new Vector3(-186f, 35f, -250f), 
            
            // 5. Cắt ngang đường băng (X=-140)
            new Vector3(-140f, 40f, -200f), 
            
            // 6. Vòng ra rìa trái an toàn để tránh tháp không lưu (X=-260)
            new Vector3(-260f, 50f, -150f),
            new Vector3(-260f, 50f, 100f),  
            
            // 7. Trở về điểm xuất phát
            new Vector3(-45f, 40f, 260f)
        };

        // Dọn dẹp object cũ
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(transform.GetChild(i).gameObject);
        }

        // Sinh Waypoint mới
        Transform[] wps = new Transform[route.Length];
        for (int i = 0; i < route.Length; i++)
        {
            GameObject wp = new GameObject($"WP_{i + 1}");
            wp.transform.SetParent(transform);
            wp.transform.position = route[i];
            wps[i] = wp.transform;
        }

        if (patrolScript != null)
        {
            patrolScript.waypoints = wps;
        }
    }
}