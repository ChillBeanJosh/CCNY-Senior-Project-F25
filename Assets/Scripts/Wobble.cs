using UnityEngine;

public class Wobble : MonoBehaviour
{
    public bool wobble;
    [SerializeField] float speed, amplitude;
    [SerializeField] Vector3 currentRot;
    [SerializeField] Vector3 startingRot;
    [SerializeField] ParticleSystem ps;
    [Space(15)]
    [Header("Rotation Axis")]
    [SerializeField] bool x;
    [SerializeField] bool y;
    [SerializeField] bool z;

    void Start()
    {
        startingRot = transform.localEulerAngles;
        currentRot = transform.localEulerAngles;
        speed = Random.Range(8f, 10f);
        amplitude = Random.Range(2f, 2.5f);
        ps.Stop();
    }

    void Update()
    {
        if (wobble)
        {
            if (!ps.isPlaying) ps.Play();

            Vector3 rot = currentRot;

            rot.x = x ? currentRot.x + Mathf.Sin(Time.time * speed) * amplitude : currentRot.x;
            rot.y = y ? currentRot.y + Mathf.Sin(Time.time * speed) * amplitude : currentRot.y;
            rot.z = z ? currentRot.z + Mathf.Sin(Time.time * speed) * amplitude : currentRot.z;

            transform.localEulerAngles = rot;
        }
        else
        {
            if (ps.isPlaying) ps.Stop();
            transform.localEulerAngles = Vector3.Slerp(transform.localEulerAngles, startingRot, 1f);
        }
    }
}
