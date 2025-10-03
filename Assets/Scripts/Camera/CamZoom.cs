using UnityEngine;

public class CamZoom : MonoBehaviour
{
    [SerializeField]
    float zoomSpeed = 6.0f, smoothness = 5.0f, min = 2.0f, max = 40.0f;

    private float currentZoom = 3.5f;
    private Camera cam;
    void Awake()
    {
        cam = GetComponentInChildren<Camera>();
    }

    void Update()
    {
        //measure how far player scrolls each frame and apply it to zoom
        currentZoom = Mathf.Clamp(value: currentZoom - Input.mouseScrollDelta.y * zoomSpeed * Time.deltaTime, min, max);

        //smooth transition when zoom changes
        cam.orthographicSize = Mathf.Lerp(a: cam.orthographicSize, b: currentZoom, t: smoothness * Time.deltaTime);
    }
}
