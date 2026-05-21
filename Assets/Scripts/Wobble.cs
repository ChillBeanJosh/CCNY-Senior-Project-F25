using UnityEngine;

public class Wobble : MonoBehaviour
{
    public bool wobble;
    [SerializeField] float speed, amplitude;
    [SerializeField] Vector3 currentRot;
    [SerializeField] Vector3 startingRot;
    [SerializeField] ParticleSystem ps;

    void Start()
    {
        startingRot = transform.localEulerAngles;
        currentRot = transform.localEulerAngles;
        speed = Random.Range(8f, 10f);
        amplitude = Random.Range(2f, 2.5f);
        ps = transform.GetChild(1).GetComponent<ParticleSystem>();
        ps.Stop();
    }

    void Update()
    {
        if (wobble)
        {
            if (!ps.isPlaying) ps.Play();

            transform.localEulerAngles = new Vector3
            (
                currentRot.x + Mathf.Sin(Time.time * speed) * amplitude,
                currentRot.y,
                currentRot.z
            );
        }
        else
        {
            if (ps.isPlaying) ps.Stop();
            transform.localEulerAngles = Vector3.Slerp(transform.localEulerAngles, startingRot, 1f);
        }
    }
}
