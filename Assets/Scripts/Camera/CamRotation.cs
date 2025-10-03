using UnityEngine;

public class CamRotation : MonoBehaviour
{
    [SerializeField] float rotSpeed = 5.0f;

    void Update()
    {
        float mouseDeltaX = Input.GetAxis("Mouse X");

        if (Input.GetMouseButton(1)) // move while holding right mouse button
            transform.Rotate(Vector3.up, angle: mouseDeltaX * rotSpeed * Time.deltaTime);
    }
}
