using UnityEngine;

public class Minimap : MonoBehaviour
{
    [Header("Target to Follow")]
    [Tooltip("Kéo thả đối tượng bạn muốn bản đồ đi theo vào đây (ví dụ: UAV hoặc Người chơi)")]
    public Transform target;

    [Header("Minimap Settings")]
    [Tooltip("Độ cao của camera Minimap so với mục tiêu")]
    public float height = 100f;
    
    [Tooltip("Nếu tick, bản đồ sẽ xoay theo hướng nhìn của mục tiêu. Nếu bỏ tick, bản đồ luôn hướng về phía Bắc.")]
    public bool rotateWithTarget = false;

    private void LateUpdate()
    {
        if (target == null) return;

        // Cập nhật vị trí: Đi theo mục tiêu trên mặt phẳng XZ, giữ độ cao cố định bằng height
        Vector3 newPosition = target.position;
        newPosition.y = target.position.y + height;
        transform.position = newPosition;

        // Cập nhật góc xoay
        if (rotateWithTarget)
        {
            transform.rotation = Quaternion.Euler(90f, target.eulerAngles.y, 0f);
        }
        else
        {
            // Luôn nhìn thẳng xuống đất, hướng cố định
            transform.rotation = Quaternion.Euler(90f, 0f, 0f);
        }
    }
}
