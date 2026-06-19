using UnityEngine;
using UnityEngine.AI;

public class MoveToTarget : MonoBehaviour
{
    //Dùng mảng để chứa nhiều điểm đến 
    public Transform[] waypoints;

    private int currentPoint = 0;
    private NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        // Bắt đầu game, cho nhân vật đi tới điểm đầu tiên trong danh sách
        if (waypoints.Length > 0)
        {
            agent.SetDestination(waypoints[currentPoint].position);
        }
    }

    void Update()
    {
        // Kiểm tra xem nhân vật đã đến nơi chưa (cách đích dưới 0.5 mét)
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            GoToNextPoint();
        }
    }

    void GoToNextPoint()
    {
        // Nếu không có điểm đến nào thì bỏ qua
        if (waypoints.Length == 0) return;

        // Chuyển sang điểm tiếp theo. Khi đi hết danh sách thì quay lại điểm số 0
        currentPoint = (currentPoint + 1) % waypoints.Length;
        agent.SetDestination(waypoints[currentPoint].position);
    }
}