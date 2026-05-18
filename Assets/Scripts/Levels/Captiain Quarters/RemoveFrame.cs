using UnityEngine;

public class RemoveFrame : MonoBehaviour
{
    float targetTime = 3f;
    Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (!rb.isKinematic && rb.constraints != RigidbodyConstraints.None)
        {
            if (targetTime > 0f)
            {
                targetTime -= Time.deltaTime;
            }
            else
            {
                rb.constraints = RigidbodyConstraints.None;
            }
        }
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.CompareTag("Ball"))
        {
            col.gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
            rb.isKinematic = false;
        }
    }
}
