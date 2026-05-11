using UnityEngine;

public class OscillateObject : MonoBehaviour
{
    [Header("Position")]
    Vector3 pos;
    float dist, moveSpeed;
    [SerializeField] float distRangeStart = 0.5f, distRangeEnd = 0.8f;
    [SerializeField] float moveSpeedRangeStart = 0.25f, moveSpeedRangeEnd = 0.8f;
    [Space(10)]
    [SerializeField] bool includeRotation;
    [Space(10)]
    [Header("Rotation")]
    Vector3 rot;
    float rotDist, rotSpeed;
    [SerializeField] float rotRangeStart = 10f, rotRangeEnd = 20f;
    [SerializeField] float rotSpeedRangeStart = 1f, rotSpeedRangeEnd = 2f;
    float time = 0f;
    bool heads;

    void Start()
    {
        pos = transform.position;
        dist = Random.Range(distRangeStart, distRangeEnd);
        moveSpeed = Random.Range(moveSpeedRangeStart, moveSpeedRangeEnd);

        rot = transform.localEulerAngles;
        rotDist = Random.Range(rotRangeStart, rotRangeEnd);
        rotSpeed = Random.Range(rotSpeedRangeStart, rotSpeedRangeEnd);

        heads = Random.Range(1, 3) == 1 ? true : false;
    }
    void Update()
    {
        time += Time.deltaTime;

        if (heads)
            transform.position = new Vector3(pos.x, pos.y + Mathf.Sin(time * moveSpeed) * dist, pos.z);
        else
            transform.position = new Vector3(pos.x, pos.y - Mathf.Sin(time * moveSpeed) * dist, pos.z);


        if (includeRotation)
            transform.localEulerAngles = new Vector3(rot.x, rot.y + Mathf.Sin(time * rotSpeed) * rotDist, rot.z);
    }
}
