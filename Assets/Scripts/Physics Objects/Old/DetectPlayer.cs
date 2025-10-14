using System.Buffers;
using UnityEngine;

public class DetectPlayer : MonoBehaviour
{
    // NO LONGER USING //
    [SerializeField] Transform moveableObj;

    void Start()
    {
        transform.parent = null;
    }


    void Update()
    {
        if (transform.parent == null) // Snap back to moveable object when empty
        {

            transform.position = moveableObj.position;
            transform.localEulerAngles = Vector3.zero;
        }
    }


    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Player")
        {
            // Link script to player that enters collider
            //col.gameObject.GetComponent<PlayerMovement>().moveableObj = this;
        }
    }

    void OnTriggerExit(Collider col)
    {
        if (col.gameObject.tag == "Player")
        {
            // Unlink script from player
            //col.gameObject.GetComponent<PlayerMovement>().moveableObj = null;
        }
    }
}
