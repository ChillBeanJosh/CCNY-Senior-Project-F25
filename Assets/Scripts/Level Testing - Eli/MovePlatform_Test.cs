using NUnit.Framework.Internal;
using UnityEngine;

public class MovePlatform_Test : MonoBehaviour
{
    [Header("Platform Settings")]
    [SerializeField] Vector3 dest;
    [SerializeField] float Speed = 0.1f;

    public bool MovePlatform = false;

    // Update is called once per frame
    void Update()
    {
        if (MovePlatform)
        transform.position = Vector3.MoveTowards(transform.position, dest, Speed);
    }
}
