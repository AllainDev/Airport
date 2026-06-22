using UnityEngine;

public class UAV_Patrol : MonoBehaviour
{
    [Header("Patrol Settings")]
    [Tooltip("Danh sách các điểm UAV sẽ bay qua theo thứ tự.")]
    public Transform[] waypoints;
    [SerializeField] private float flySpeed = 15f;
    [SerializeField] private float rotationSpeed = 2f;
    [Tooltip("Khoảng cách tối thiểu để xác nhận UAV đã đến waypoint.")]
    [SerializeField] private float waypointTolerance = 1.5f;

    private int currentWaypointIndex = 0;
    private bool hasPath = false;

    private void Start()
    {
        if (waypoints != null && waypoints.Length > 0)
        {
            hasPath = true;
            // Nếu muốn UAV không dính chặt vào object gốc, hãy đảm bảo object gốc không bị khóa Rigidbody kinematic nếu có.
        }
        else
        {
            Debug.LogWarning("UAV_Patrol: Chưa gán Waypoints! UAV sẽ không di chuyển.");
        }
    }

    private void Update()
    {
        if (!hasPath) return;

        Patrol();
    }

    private void Patrol()
    {
        Transform targetWaypoint = waypoints[currentWaypointIndex];

        // 1. Tính toán hướng bay
        Vector3 directionToTarget = targetWaypoint.position - transform.position;

        // 2. Xử lý xoay mũi UAV (mượt mà)
        if (directionToTarget != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        // 3. Di chuyển tiến lên phía trước theo hướng mũi UAV đang chỉ
        // Dùng transform.forward để UAV bay theo hướng nó đang nhìn, tạo cảm giác bay thật hơn là dịch chuyển tịnh tiến.
        transform.position += transform.forward * flySpeed * Time.deltaTime;

        // 4. Kiểm tra xem đã đến gần Waypoint chưa
        if (Vector3.Distance(transform.position, targetWaypoint.position) <= waypointTolerance)
        {
            // Chuyển sang điểm tiếp theo, nếu đến điểm cuối thì quay lại điểm đầu (vòng lặp kín)
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
        }
    }

    // Hàm này giúp vẽ đường bay trong cửa sổ Scene (không hiện trong Game) để bạn dễ setup map.
    private void OnDrawGizmos()
    {
        if (waypoints == null || waypoints.Length < 2) return;

        Gizmos.color = Color.cyan;
        for (int i = 0; i < waypoints.Length; i++)
        {
            if (waypoints[i] == null) continue;

            // Vẽ quả cầu đánh dấu Waypoint
            Gizmos.DrawWireSphere(waypoints[i].position, waypointTolerance);

            // Vẽ đường nối giữa các Waypoint
            int nextIndex = (i + 1) % waypoints.Length;
            if (waypoints[nextIndex] != null)
            {
                Gizmos.DrawLine(waypoints[i].position, waypoints[nextIndex].position);
            }
        }
    }
}