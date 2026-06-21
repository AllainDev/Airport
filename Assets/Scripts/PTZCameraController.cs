using UnityEngine;
using UnityEngine.InputSystem; 

public class PTZCameraController : MonoBehaviour
{
    public Transform panAxis;   
    public Transform pitchAxis; 
    
    [Header("Hardware Constraints")]
    public float lockedHeight = 100f;
    
    [Tooltip("Giới hạn góc gật gù (Pitch)")]
    public float minPitch = -90f; // Góc cắm mặt xuống đất
    public float maxPitch = 0f;   // Góc ngẩng ngang tầm mắt
    
    [Header("PTZ Controls (For Testing)")]
    public float panSpeed = 20f;
    public float pitchSpeed = 20f;

    [Header("Zoom Settings")]
    public float zoomSpeed = 50f; 
    public float minFOV = 0.1f;    
    public float maxFOV = 60f;    

    private Camera[] childCameras;

    [Header("Smoothing (Làm mượt)")]
    public float smoothTime = 0.2f; // Thời gian trễ tạo quán tính (mô-tơ)

    // Biến lưu "Góc đích" (Camera sẽ lướt êm ái tới đây)
    private float targetPan = 0f;
    private float targetPitch = -30f;

    // Biến lưu "Góc hiện tại" đang đứng
    private float currentPan = 0f;
    private float currentPitch = -30f;

    // Biến gia tốc bắt buộc dùng cho hàm SmoothDamp của Unity
    private float panVelocity = 0f;
    private float pitchVelocity = 0f;

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

        //Smooth
        // Thay vì cộng thẳng làm camera quay giật cục, ta cộng dồn vào "Góc đích"
        targetPan += panInput * panSpeed * Time.deltaTime;
        targetPitch += pitchInput * pitchSpeed * Time.deltaTime;
        targetPitch = Mathf.Clamp(targetPitch, minPitch, maxPitch);
        // 3. THUẬT TOÁN LÀM MƯỢT: Kéo thấu kính từ từ lướt tới "Góc đích"
        currentPan = Mathf.SmoothDampAngle(currentPan, targetPan, ref panVelocity, smoothTime);
        currentPitch = Mathf.SmoothDamp(currentPitch, targetPitch, ref pitchVelocity, smoothTime);


        if (panAxis != null)
        {
            // Ép góc quay bằng đúng góc currentPan đã làm mượt
            panAxis.localEulerAngles = new Vector3(0f, currentPan, 0f);
        }
        
        if (pitchAxis != null)
        {
            // Gọi hàm ApplyPitch để lấy currentPitch đã làm mượt áp dụng vào
            ApplyPitch();
        }

        if (Mouse.current != null)
        {
            float scroll = Mouse.current.scroll.y.ReadValue();
            
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


    // Hàm nhận dữ liệu góc từ AI Laser truyền sang
    public void SetTargetAngle(float pan, float pitch)
    {
        targetPan = pan;
        targetPitch = pitch;
    }

    // Hàm nhận lệnh Auto-Zoom từ AI Tracking
    public void SetZoom(float fov)
    {
        foreach (Camera cam in childCameras)
        {
            cam.fieldOfView = Mathf.Clamp(fov, minFOV, maxFOV);
        }
    }
}
