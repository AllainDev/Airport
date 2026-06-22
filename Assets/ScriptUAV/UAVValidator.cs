using UnityEngine;

public class UAVValidator : MonoBehaviour
{
    public Camera cam;
    public Renderer uav;

    void Update()
    {
        if (cam == null || uav == null || !uav.isVisible) return;

        Bounds b = uav.bounds;
        Vector3 c = b.center, e = b.extents;
        Vector3[] pts = {
            cam.WorldToScreenPoint(c + new Vector3(e.x, e.y, e.z)),
            cam.WorldToScreenPoint(c + new Vector3(e.x, e.y, -e.z)),
            cam.WorldToScreenPoint(c + new Vector3(e.x, -e.y, e.z)),
            cam.WorldToScreenPoint(c + new Vector3(e.x, -e.y, -e.z)),
            cam.WorldToScreenPoint(c + new Vector3(-e.x, e.y, e.z)),
            cam.WorldToScreenPoint(c + new Vector3(-e.x, e.y, -e.z)),
            cam.WorldToScreenPoint(c + new Vector3(-e.x, -e.y, e.z)),
            cam.WorldToScreenPoint(c + new Vector3(-e.x, -e.y, -e.z))
        };

        float minX = pts[0].x, maxX = pts[0].x, minY = pts[0].y, maxY = pts[0].y;
        foreach (Vector3 p in pts)
        {
            if (p.x < minX) minX = p.x; if (p.x > maxX) maxX = p.x;
            if (p.y < minY) minY = p.y; if (p.y > maxY) maxY = p.y;
        }

        float pct = ((maxX - minX) * (maxY - minY)) / (Screen.width * Screen.height) * 100f;
        float dist = Vector3.Distance(cam.transform.position, uav.transform.position);

        if (pct >= 1f && dist > 1000f)
        {
            Debug.LogError($"FAILED: UAV chiếm {pct:F2}% (>= 1%) khung hình ở cự ly {dist:F0}m (>1km).");
        }
    }
}