using System.Security.Cryptography;
using UnityEngine;

public class MoveableObject : MonoBehaviour
{
    [SerializeField] Transform target;
    Rigidbody rb;
    Collider col;
    [SerializeField] PhysicsMaterial pm;


    void Start()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
    }


    void FixedUpdate()
    {
        Movement();
    }

    void Update()
    {
        if (target.transform.parent == null)
        {
            rb.mass = 50.0f;
            col.material = pm;
        }
        else
        {
            if (rb.mass != 0.0f) rb.mass = 0.0f;
            col.material = null;
        }
    }

    void Movement()
    {
        float dist = Mathf.Abs(target.position.x - this.transform.position.x);

        if (dist > 0.05f)
        {
            Vector3 dir = (target.position - transform.position).normalized;
            rb.linearVelocity = Vector3.Lerp(rb.linearVelocity, dir * 10.0f, Time.fixedDeltaTime * 5.0f);
        }
        else
        {
            rb.linearVelocity = Vector3.Lerp(rb.linearVelocity, Vector3.zero, Time.fixedDeltaTime);
        }
    }
}
