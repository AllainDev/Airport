using UnityEngine;
using UnityEngine.InputSystem;

public class CameraTracker : MonoBehaviour
{
    private PTZCameraController ptzController;
    private Transform target;
    private int currentTargetIndex = -1;

    [Header("Tinh chỉnh Camera (Có thể sửa trực tiếp trong Unity)")]
    [Tooltip("Khoảng cách nâng tâm ngắm lên cao (để không bị ngắm vào chân)")]
    public float targetHeightOffset = 1.5f;
    
    [Tooltip("Khung hình bao quát bao nhiêu mét? (Số càng to thì Zoom càng ít)")]
    public float frameHeight = 25.0f; 
    
    [Tooltip("Độ Zoom tối đa (FOV nhỏ nhất). Không để quá nhỏ tránh văng mục tiêu ra ngoài")]
    public float maxZoomFOV = 35f;

    void Start()
    {
        ptzController = GetComponent<PTZCameraController>();
    }

    void Update()
    {
        // 1. Nhấn phím T để xoay vòng mục tiêu + Auto Zoom
        if (Keyboard.current.tKey.wasPressedThisFrame)
        {
            CycleNextTarget();
        }

        // 2. Nhấn phím ESC để hủy khóa
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            target = null;
        }

        // 3. Liên tục bám theo
        if (target != null && ptzController != null)
        {
            TrackTarget();
        }
    }

    void CycleNextTarget()
    {
        MoveToTarget[] people = Object.FindObjectsByType<MoveToTarget>(FindObjectsSortMode.None);
        if (people.Length == 0)
        {
            return;
        }

        currentTargetIndex++;
        if (currentTargetIndex >= people.Length) currentTargetIndex = 0;

        target = people[currentTargetIndex].transform;
        AutoZoom();
    }

    void AutoZoom()
    {
        float dist = Vector3.Distance(transform.position, target.position);
        float autoFOV = 2.0f * Mathf.Atan(frameHeight / (2.0f * dist)) * Mathf.Rad2Deg;
        
        // Chặn không cho zoom quá đáng
        autoFOV = Mathf.Max(autoFOV, maxZoomFOV);
        
        ptzController.SetZoom(autoFOV);
    }

    void TrackTarget()
    {
        Vector3 aimPoint = target.position + new Vector3(0f, targetHeightOffset, 0f);
        Vector3 localTargetPos = transform.InverseTransformPoint(aimPoint);

        float targetPan = Mathf.Atan2(localTargetPos.x, localTargetPos.z) * Mathf.Rad2Deg;
        float horizontalDistance = Mathf.Sqrt(localTargetPos.x * localTargetPos.x + localTargetPos.z * localTargetPos.z);
        
        // BỎ DẤU TRỪ: PTZCameraController dùng góc âm (-60 đến -30) để ngó xuống, nên ta truyền thẳng góc âm vào.
        float targetPitch = Mathf.Atan2(localTargetPos.y, horizontalDistance) * Mathf.Rad2Deg;

        ptzController.SetTargetAngle(targetPan, targetPitch);
    }
}