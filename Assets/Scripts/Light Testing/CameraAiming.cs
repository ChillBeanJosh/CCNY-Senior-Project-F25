using UnityEngine;

public class CameraAiming : MonoBehaviour
{
    public Camera mainCamera;
    public float rotationSpeed = 10f;

    void Update()
    {
        //Camera Refrence:
        if (mainCamera == null) mainCamera = Camera.main;

        //Mouse Position in Screen Space:
        Vector3 mousePos = Input.mousePosition;

        //Rotated Object Position:
        Vector3 objPos = transform.position;

        //Mouse Position, Relative to Camera:
        Vector3 worldMousePos = mainCamera.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, mainCamera.WorldToScreenPoint(objPos).z));

        //Direction from Mouse to Object:
        Vector3 direction = worldMousePos - objPos;

        //Set Z to 0 for 2.5D:
        direction.z = 0;

        if (direction.sqrMagnitude > 0.001f)
        {
            //Target Rotation, using Direction:
            Quaternion targetRotation = Quaternion.LookRotation(Vector3.forward, direction);

            //Rotation Lerping:
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

}
