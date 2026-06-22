using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class UAVController : MonoBehaviour
{
    [Header("Movement Physics")]
    [SerializeField] private float moveForce = 150f;
    [SerializeField] private float rotationSpeed = 100f;
    [SerializeField] private float dragBase = 4f;

    [Header("Visual Settings")]
    [SerializeField] private Transform visualModel;
    [SerializeField] private float tiltAngle = 15f;
    [SerializeField] private float tiltSmoothness = 5f;

    [Header("Fan System")]
    [SerializeField] private FanRotation[] fans;

    private Rigidbody rb;
    private Vector3 moveInput;
    private float mouseX;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.linearDamping = dragBase;
        rb.angularDamping = dragBase;
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        if (visualModel == this.transform) visualModel = null;
    }

    private void Start()
    {
        SetActiveFans(true);
    }

    private void Update()
    {
        if (Keyboard.current == null || Mouse.current == null) return;

        bool isHoldingRightClick = Mouse.current.rightButton.isPressed;
        if (isHoldingRightClick)
        {
            Cursor.lockState = CursorLockMode.Locked;
            mouseX = Mouse.current.delta.x.ReadValue();
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            mouseX = 0f;
        }

        float moveX = (Keyboard.current.dKey.isPressed ? 1f : 0f) - (Keyboard.current.aKey.isPressed ? 1f : 0f);
        float moveZ = (Keyboard.current.wKey.isPressed ? 1f : 0f) - (Keyboard.current.sKey.isPressed ? 1f : 0f);
        float moveY = Keyboard.current.spaceKey.isPressed ? 1f : Keyboard.current.leftShiftKey.isPressed ? -1f : 0f;

        moveInput = new Vector3(moveX, moveY, moveZ).normalized;
        HandleVisualTilt();
    }

    private void FixedUpdate()
    {
        Vector3 flatForward = transform.forward; flatForward.y = 0; flatForward.Normalize();
        Vector3 flatRight = transform.right; flatRight.y = 0; flatRight.Normalize();
        Vector3 forceDirection = flatRight * moveInput.x + Vector3.up * moveInput.y + flatForward * moveInput.z;

        rb.AddForce(forceDirection * moveForce, ForceMode.Acceleration);

        if (Mathf.Abs(mouseX) > 0.01f)
        {
            Quaternion turnRotation = Quaternion.Euler(0f, mouseX * rotationSpeed * Time.fixedDeltaTime, 0f);
            rb.MoveRotation(rb.rotation * turnRotation);
        }
    }

    private void HandleVisualTilt()
    {
        if (visualModel == null) return;
        float targetTiltX = moveInput.z * tiltAngle;
        float targetTiltZ = -moveInput.x * tiltAngle;
        Quaternion targetRotation = Quaternion.Euler(targetTiltX, 0, targetTiltZ);
        visualModel.localRotation = Quaternion.Slerp(visualModel.localRotation, targetRotation, Time.deltaTime * tiltSmoothness);
    }

    private void SetActiveFans(bool state)
    {
        if (fans == null) return;
        foreach (var fan in fans) { if (fan != null) fan.isActive = state; }
    }
}