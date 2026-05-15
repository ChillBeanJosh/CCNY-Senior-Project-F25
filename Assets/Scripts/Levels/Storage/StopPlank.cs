using UnityEngine;

public class StopPlank : MonoBehaviour
{
    Rigidbody rb;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.CompareTag("Obstruction"))
        {
            rb.isKinematic = true;
        }
    }
}
