using UnityEngine;

public class GrabObject : MonoBehaviour
{
    public Rigidbody rb;
    Collider col;

    // No friction
    [SerializeField] PhysicsMaterial pm;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
    }


    void Update()
    {
        // Add friction when player is moving object
        if (transform.parent != null)
            col.material = null;
        else if (col.material == null)
            col.material = pm;
    }


    void OnCollisionEnter(Collision col)
    {
        // Asign script to player on collision
        if (col.gameObject.tag == "Player") col.gameObject.GetComponent<PlayerMovement>().grab = this;
    }

    void OnCollisionExit(Collision col)
    {
        // Remove script when player lets go
        if (col.gameObject.tag == "Player" && transform.parent == null) col.gameObject.GetComponent<PlayerMovement>().grab = null;
    }
}
