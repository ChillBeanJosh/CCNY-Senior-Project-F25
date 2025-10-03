using System.Xml.Schema;
using UnityEngine;

public class CamPan : MonoBehaviour
{
    [SerializeField]
    float speed = 6.0f;
    [SerializeField]
    Vector2 limitX, limitZ;
    private Camera cam;
    void Awake()
    {
        cam = GetComponentInChildren<Camera>();
    }

    void Update()
    {
        Vector2 panPos = new Vector2(x: Input.GetAxis("Horizontal"), y: Input.GetAxis("Vertical"));

        //Euler ensures cam direction based on direction it faces
        transform.position += Quaternion.Euler(x: 0, y: cam.transform.eulerAngles.y, z: 0f) * new Vector3(x: panPos.x, y: 0f, z: panPos.y) * speed * Time.deltaTime;

        //clamp panning
        transform.position = new Vector3(x: Mathf.Clamp(value: transform.position.x, min: limitX.x, max: limitX.y), y: transform.position.y,
            z: Mathf.Clamp(value: transform.position.z, min: limitZ.x, max: limitZ.y));
    }
}
