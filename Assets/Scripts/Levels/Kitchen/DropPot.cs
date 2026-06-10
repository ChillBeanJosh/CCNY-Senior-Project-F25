using UnityEngine;
using UnityEngine.Events;

public class DropPot : MonoBehaviour
{
    Rigidbody rb;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void Drop()
    {
        rb.isKinematic = false;
    }
}
