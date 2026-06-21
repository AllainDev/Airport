using UnityEngine;
using UnityEngine.InputSystem; 

public class PTZCameraController : MonoBehaviour
{
    public Transform panAxis;   
    public Transform pitchAxis; 
    
    [Header("Hardware Constraints")]
    public float lockedHeight = 100f;
    
    [Tooltip("Khóa cứng theo đúng yêu cầu đề bài")]
    public float minPitch = -60f;
    public float maxPitch = -30f;
    
    [Header("PTZ Controls (For Testing)")]
    public float panSpeed = 20f;
    public float pitchSpeed = 20f;

    [Header("Zoom Settings")]
    public float zoomSpeed = 50f; 
    public float minFOV = 10f;    
    public float maxFOV = 60f;    
    
    // Bắt đầu ở góc ngẩng cao nhất có thể trong giới hạn cho phép
    private float currentPitch = -30f; 

    private Camera[] childCameras;
    
    void Start()
    {
        currentPitch = Mathf.Clamp(currentPitch, minPitch, maxPitch);
        ApplyPitch();
        
        childCameras = GetComponentsInChildren<Camera>();
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

        if (UnityEngine.InputSystem.Mouse.current != null)
        {
            float scroll = UnityEngine.InputSystem.Mouse.current.scroll.y.ReadValue();
            
            if (Mathf.Abs(scroll) > 0.01f) // Chỉ chạy nếu có lăn chuột
            {
                foreach (Camera cam in childCameras)
                {
                    float targetFOV = cam.fieldOfView - (scroll * zoomSpeed * Time.deltaTime);
                    cam.fieldOfView = Mathf.Clamp(targetFOV, minFOV, maxFOV);
                }
            }
        }
    }

    void ApplyPitch()
    {
        // Đảo ngược trục để số âm thành góc chúi xuống
        pitchAxis.localEulerAngles = new Vector3(-currentPitch, 0f, 0f);
    }
}
