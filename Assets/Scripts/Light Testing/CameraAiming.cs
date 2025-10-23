using UnityEngine;

public class CameraAiming : MonoBehaviour
{
    public Camera mainCamera;
    public float rotationSpeed = 10f;
    public PlayerMovement playerMovement;

    void Update()
    {
        //Camera Refrence:
        if (mainCamera == null) mainCamera = Camera.main;

        //Mouse Position in Screen Space:
        //Vector3 mousePos = Input.mousePosition;

        //Rotated Object Position:
        //Vector3 objPos = transform.position;

        //Mouse Position, Relative to Camera:
        //Vector3 worldMousePos = mainCamera.ScreenToWorldPoint(new Vector3(mousePos.x, mainCamera.WorldToScreenPoint(objPos).y, mousePos.z));

        //Direction from Mouse to Object:
        Vector3 direction = mainCamera.transform.forward;
       

        //Set Z to 0 for 2.5D:
        //direction.y = 0;

        if (direction.sqrMagnitude > 0.001f)
        {
            //Target Rotation, using Direction:
            Quaternion targetRotation = Quaternion.LookRotation(mainCamera.transform.up, mainCamera.transform.forward);

            //Rotation Lerping:
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

}
