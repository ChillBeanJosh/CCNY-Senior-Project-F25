using System;
using UnityEngine;

public class RisingPlatforms : MonoBehaviour
{
    [Header("References")]
    public GameObject[] RisingPillar;
    Collider ThisCollider;
    

    [Header("Settings")]
    public float hitRadius = 0.5f;
    private LightReflection[] allLasers;
    

    private void Start()
    {
        allLasers = FindObjectsByType<LightReflection>(FindObjectsInactive.Include, FindObjectsSortMode.None);
         ThisCollider = GetComponent<Collider>();


    }

    void LateUpdate()
    {
        if (allLasers == null || allLasers.Length == 0) return;

        bool isBottomHit = false;

        foreach (var laser in allLasers)
        {
            if (laser == null) continue;
            if (laser.gameObject.name.Contains("Point_")) continue;
            if (laser.laserPoints == null || laser.laserPoints.Count == 0) continue;

            foreach (Vector3 point in laser.laserPoints)
            {
                if (!isBottomHit && ThisCollider != null)
                    if (Vector3.Distance(point, ThisCollider.ClosestPoint(point)) <= hitRadius)
                        isBottomHit = true;
            }
        }


        // Open door once bottom gem is hit
        if (isBottomHit)
        {
            //Activate all rising pillars
            foreach (GameObject pillar in RisingPillar)
            {
                pillar.GetComponent<PillarTarget>().PillarActive = true;
            }
                
            Debug.Log("Rising pillar activated!");
        }
    }
}