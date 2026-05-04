using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PedestalRotation : MonoBehaviour
{
    [Header("RotationSettings")] 
    public float degreesPerHit = 120f;
    public float rotationSpeed;
    
    private bool rotationInProgress = false;

    public void TriggerRotation()
    {
        if (!rotationInProgress )
        {
            Debug.Log("Rotation triggered");
            StartCoroutine(Turn());
        }
        else
        {
            Debug.Log("Rotation already in progress");
        }
    }

    IEnumerator Turn()
    {
        rotationInProgress = true;
        Debug.Log("Rotation started");
        Quaternion startRotation = transform.rotation;
        Quaternion targetRotation = startRotation * Quaternion.Euler(0, degreesPerHit, 0);
        
        

        while (Quaternion.Angle(transform.rotation, targetRotation) > 0.1f)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            yield return null;
        }
        
        transform.rotation = targetRotation;
        rotationInProgress = false;
        Debug.Log("Rotation finished");
    }
}
