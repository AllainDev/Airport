using UnityEngine;
using UnityEngine.InputSystem; 

public class PTZCameraController : MonoBehaviour
{
    [Header("Rig Axes")]
    public Transform panAxis;   
    public Transform pitchAxis; 
    
    [Header("Hardware Constraints")]
    public float lockedHeight = 25f;
    
    [Tooltip("Khóa cứng theo đúng yêu cầu đề bài")]
    public float minPitch = -60f;
    public float maxPitch = -30f;
    
    [Header("PTZ Controls (For Testing)")]
    public float panSpeed = 20f;
    public float pitchSpeed = 20f;
    
    // Bắt đầu ở góc ngẩng cao nhất có thể trong giới hạn cho phép
    private float currentPitch = -30f; 
    
    void Start()
    {
        currentPitch = Mathf.Clamp(currentPitch, minPitch, maxPitch);
        ApplyPitch();
    }
    
    void LateUpdate()
    {
        // 1. Khóa cao độ 25m
        Vector3 lockedPosition = transform.position;
        lockedPosition.y = lockedHeight;
        transform.position = lockedPosition;
        
        float panInput = 0f;
        float pitchInput = 0f;

        if (Keyboard.current != null)
        {
            if (Keyboard.current.leftArrowKey.isPressed) panInput = -1f;
            if (Keyboard.current.rightArrowKey.isPressed) panInput = 1f;

            // Bấm Lên = Ngẩng (tăng dần về -30), Bấm Xuống = Chúi (giảm dần về -60)
            if (Keyboard.current.upArrowKey.isPressed) pitchInput = 1f; 
            if (Keyboard.current.downArrowKey.isPressed) pitchInput = -1f; 
        }
        
        if (panAxis != null)
        {
            panAxis.Rotate(Vector3.up * panInput * panSpeed * Time.deltaTime);
        }
        
        if (pitchAxis != null)
        {
            currentPitch += pitchInput * pitchSpeed * Time.deltaTime;
            currentPitch = Mathf.Clamp(currentPitch, minPitch, maxPitch);
            ApplyPitch();
        }
    }

    void ApplyPitch()
    {
        // Đảo ngược trục để số âm (-60 đến -30) trở thành góc chúi xuống đất (60 đến 30 độ)
        pitchAxis.localEulerAngles = new Vector3(-currentPitch, 0f, 0f);
    }
}
