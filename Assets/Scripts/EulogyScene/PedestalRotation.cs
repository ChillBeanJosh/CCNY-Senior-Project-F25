using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PedestalRotation : MonoBehaviour
{
    [Header("RotationSettings")] 
    public float degreesPerHit = 120f;
    public float rotationDuration = 1f;
    
    private bool rotationInProgress = false;

    public void TriggerRotation()
    {
        if (!rotationInProgress )
        {
            Debug.Log("Rotation triggered");
            StartCoroutine(Turn());
        }
    }

    IEnumerator Turn()
    {
        rotationInProgress = true;
        Quaternion startRotation = transform.rotation;
        Quaternion targetRotation = startRotation * Quaternion.Euler(0, degreesPerHit, 0);
        float elapsedTime = 0f;
        
        Debug.Log("Rotation started");

        while (elapsedTime < rotationDuration)
        {
            float t = elapsedTime / rotationDuration;
            transform.rotation = Quaternion.Lerp(startRotation, targetRotation, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        transform.rotation = targetRotation;
        rotationInProgress = false;
    }
}
