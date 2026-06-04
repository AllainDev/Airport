using UnityEngine;

public class FanRotation : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 1500f;

    public bool isActive = false;

    private void Update()
    {
        if (!isActive) return;

        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime, Space.Self);
    }
}