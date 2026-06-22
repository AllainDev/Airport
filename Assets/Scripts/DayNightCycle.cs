using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    [Header("Sun Settings")]
    [Tooltip("Kéo thả ánh sáng Directional Light (Mặt trời) của bạn vào đây")]
    public Transform sunTransform;

    [Header("Time Settings")]
    [Tooltip("Tốc độ trôi qua của thời gian. Số càng lớn ngày đêm trôi càng nhanh.")]
    public float rotationSpeed = 2f; 

    void Update()
    {
        if (sunTransform != null)
        {
            // Xoay mặt trời quanh trục X theo thời gian thực
            // Khi mặt trời quay xuống dưới mặt đất (âm độ), trời sẽ tự động chuyển tối
            sunTransform.Rotate(Vector3.right * rotationSpeed * Time.deltaTime);
        }
    }
}
