using NUnit.Framework.Internal;
using System.Collections.Generic;
using UnityEngine;

public class PillarTarget : MonoBehaviour
{
    [SerializeField] Vector3 OriginalPosition;
    [SerializeField] Vector3 TargetPosition;

    [Header("Speed Settings")]
    [SerializeField] float Speed = 0.1f;

    [Header("Interval Settings")]
    [SerializeField] bool HasTimer = true;
    [SerializeField] float TimeUntilReturn = 6f;
    float timeUntilReturn;

    [Header("Rider Settings")]
    [SerializeField] bool HasRiders = true;

    public bool PillarActive = false;

    void Start()
    {
        timeUntilReturn = TimeUntilReturn;
        transform.position = OriginalPosition;
    }

    private void Update()
    {
        if (Vector3.Distance(transform.position, TargetPosition) < 0.01f && HasTimer)
        {
            timeUntilReturn -= Time.deltaTime;
            if (timeUntilReturn < 0)
            {
                PillarActive = false;
                timeUntilReturn = TimeUntilReturn;
            }
        }

        if (PillarActive)
        {
            transform.position = Vector3.MoveTowards(transform.position, TargetPosition, Speed);
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, OriginalPosition, Speed);
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (HasRiders && other.gameObject.CompareTag("Player"))
        {
            other.transform.SetParent(this.transform);
        }
    }

    private void OnCollisionExit(Collision other)
    {
        if (HasRiders && other.gameObject.CompareTag("Player"))
        {
            other.transform.SetParent(null);
        }
    }

    public void ActivatePillar()
    {
        PillarActive = true;
        timeUntilReturn = TimeUntilReturn;
    }
}
