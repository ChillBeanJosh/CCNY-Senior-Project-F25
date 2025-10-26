using UnityEngine;

public class CameraAiming : MonoBehaviour
{
    public Camera mainCamera;
    public float rotationSpeed = 10f;
    public Vector3 rotationOffset = new Vector3(-90f, 0f, 0f);

    void Update()
    {
        //Camera Refrence:
        if (mainCamera == null) mainCamera = Camera.main;

        //Get camera forward direction:
        Vector3 targetDirection = mainCamera.transform.forward;
        if (targetDirection.sqrMagnitude < 0.001f) return;

        //Target Rotation Being the mainCamera's Rotation:
        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);

        //Apply Rotation Offset: (NEEDED TO PROPERLY ALLIGN AIMING TO CAMERA)
        targetRotation *= Quaternion.Euler(rotationOffset);

        //Rotation Lerp:
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

    }
}