using System;
using UnityEngine;

public class DoorActivate : MonoBehaviour
{
    [Header("References")] 
    public Collider topGemCollider;
    public Collider bottomGemCollider;
    public GameObject bottomGemObject;
    public Animator doorAnimator;
    public string openTrigger = "DoorSwing";
    
    [Header("Settings")]
    public float hitRadius = 0.5f;
    private bool doorOpened = false;
    private LightReflection[] allLasers;
    private bool bottomGemRevealed = false;
    private bool bottomGemHit = false;

    private void Start()
    {
        allLasers = FindObjectsByType<LightReflection>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        
        if (bottomGemObject != null) bottomGemObject.SetActive(false);
    }
    
    void LateUpdate()
{
    if (doorOpened) return;
    if (allLasers == null || allLasers.Length == 0) return;

    bool isTopHit = false;
    bool isBottomHit = false;

    foreach (var laser in allLasers)
    {
        if (laser == null) continue;
        if (laser.gameObject.name.Contains("Point_")) continue;
        if (laser.laserPoints == null || laser.laserPoints.Count == 0) continue;

        foreach (Vector3 point in laser.laserPoints)
        {
            if (!isTopHit && topGemCollider != null)
                if (Vector3.Distance(point, topGemCollider.ClosestPoint(point)) <= hitRadius)
                    isTopHit = true;

            if (!isBottomHit && bottomGemCollider != null
                && bottomGemObject != null && bottomGemObject.activeInHierarchy)
                if (Vector3.Distance(point, bottomGemCollider.ClosestPoint(point)) <= hitRadius)
                    isBottomHit = true;
        }
    }

    // Reveal bottom gem once top is hit
    if (isTopHit && !bottomGemRevealed)
    {
        bottomGemRevealed = true;
        if (bottomGemObject != null) bottomGemObject.SetActive(true);
    }

    // Debug bottom gem distance
    if (bottomGemRevealed && bottomGemObject != null && bottomGemObject.activeInHierarchy)
    {
        foreach (var laser in allLasers)
        {
            if (laser == null) continue;
            if (laser.gameObject.name.Contains("Point_")) continue;
            if (laser.laserPoints == null || laser.laserPoints.Count == 0) continue;

            float closest = float.MaxValue;
            foreach (Vector3 point in laser.laserPoints)
            {
                float dist = Vector3.Distance(point, bottomGemCollider.ClosestPoint(point));
                if (dist < closest) closest = dist;
            }
            Debug.Log($"[Door] Closest to bottom gem: {closest}, hitRadius: {hitRadius}, laser: {laser.gameObject.name}");
        }
    }

    // Open door once bottom gem is hit
    if (isBottomHit && !bottomGemHit)
    {
        bottomGemHit = true;
        doorOpened = true;
        Debug.Log($"[Door] Opening door, firing trigger: {openTrigger}");
        doorAnimator.SetTrigger(openTrigger);
    }
}
}