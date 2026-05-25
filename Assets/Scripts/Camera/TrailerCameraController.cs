using UnityEngine;
using Unity.Cinemachine;

public class TrailerCameraController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float sprintMultiplier = 2f;
    [SerializeField] private float verticalSpeed = 8f;

    [Header("Look Settings")]
    [SerializeField] private float mouseSensitivity = 2f;
    [SerializeField] private float maxLookAngle = 90f;

    [Header("References")]
    [SerializeField] private CinemachineCamera cinemachineCamera;

    private float pitch = 0f;
    private float yaw = 0f;
    private bool isActive = false;

    void Start()
    {
        // If no camera assigned, try to find one on this object
        if (cinemachineCamera == null)
        {
            cinemachineCamera = GetComponent<CinemachineCamera>();
        }
        
        // Initialize yaw and pitch to current rotation
        Vector3 angles = transform.eulerAngles;
        yaw = angles.y;
        pitch = angles.x;
        if (pitch > 180) pitch -= 360;
    }

    void Update()
    {
        if (!isActive) return;

        HandleMouseLook();
        HandleMovement();
    }

    public void SetActive(bool active)
    {
        isActive = active;
        if (isActive)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            
            // Sync rotation to current transform when activating
            Vector3 angles = transform.eulerAngles;
            yaw = angles.y;
            pitch = angles.x;
            if (pitch > 180) pitch -= 360;

            if (cinemachineCamera != null)
            {
                cinemachineCamera.Priority = 100; // High priority to take over
            }
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            
            if (cinemachineCamera != null)
            {
                cinemachineCamera.Priority = 0; // Low priority to return control
            }
        }
    }

    void HandleMovement()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 moveDirection = transform.right * horizontal + transform.forward * vertical;

        float verticalMovement = 0f;
        if (Input.GetKey(KeyCode.Space)) verticalMovement = 1f;
        if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.C)) verticalMovement = -1f;

        moveDirection += Vector3.up * verticalMovement;

        float currentSpeed = moveSpeed;
        if (Input.GetKey(KeyCode.LeftShift))
        {
            currentSpeed *= sprintMultiplier;
        }

        transform.position += moveDirection.normalized * currentSpeed * Time.deltaTime;
    }

    void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        yaw += mouseX;
        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, -maxLookAngle, maxLookAngle);

        transform.rotation = Quaternion.Euler(pitch, yaw, 0f);
    }
}
