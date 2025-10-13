using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    // NOT USED
    float xRotation;
    float yRotation;
    [SerializeField] float sens;
    [SerializeField] Transform orientation;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        CameraControls();
    }

    void LateUpdate()
    {
        // Rotate cam and player
        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        orientation.rotation = Quaternion.Euler(0, yRotation, 0);
    }

    void CameraControls()
    {
        float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * sens;
        float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * sens;

        yRotation += mouseX;
        xRotation -= mouseY;

        // Ensures player can't look 360 degrees up and down
        xRotation = Mathf.Clamp(xRotation, -90.0f, 90.0f);
    }
}
