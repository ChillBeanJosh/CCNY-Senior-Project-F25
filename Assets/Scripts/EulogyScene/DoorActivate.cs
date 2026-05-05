using System;
using UnityEngine;

public class DoorActivate : MonoBehaviour
{
    [Header("Gems")] 
    public Collider topGemCollider;
    public Collider bottomGemCollider;
    public GameObject bottomGemObject;

    [Header("Door Animations")] 
    public Animator doorAnimator;
    public string openTrigger = "DoorSwing";
    
    public float hitRadius = 0.5f;
    private bool doorOpened = false;
    private LightReflection[] allLasers;

    private void Start()
    {
        RefreshLasers();
    }

    public void RefreshLasers()
    {
        allLasers = FindObjectsByType<LightReflection>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
    }

    private void LateUpdate()
    {
        if (doorOpened) return;
        RefreshLasers();
        
        bool isTopHit = false;
        bool isBottomHit = false;

        foreach (var laser in allLasers)
        {
            if (laser.gameObject.name.Contains("Point_")) continue;
            if (laser.laserPoints == null || laser.laserPoints.Count == 0) continue;

            foreach (Vector3 point in laser.laserPoints)
            {
                if (!isTopHit && topGemCollider != null)
                {
                    if (Vector3.Distance(point, topGemCollider.ClosestPoint(point)) <= hitRadius)
                        isTopHit = true;
                }

                if (!isBottomHit && bottomGemCollider != null && bottomGemObject.activeInHierarchy && bottomGemObject != null)
                {
                    if (Vector3.Distance(point, bottomGemCollider.ClosestPoint(point)) <= hitRadius)
                    {
                        isBottomHit = true;
                    }
                }

                if (isTopHit && isBottomHit) break;
            }
            
            if (bottomGemObject != null)
            {
                bool shouldBeVisible = isTopHit;
                if (bottomGemObject.activeInHierarchy != shouldBeVisible)
                    bottomGemObject.SetActive(shouldBeVisible);
            }

            if (isTopHit && isBottomHit)
            {
                doorOpened = true;
                doorAnimator.SetTrigger(openTrigger);
            }
        }
        
    }
}