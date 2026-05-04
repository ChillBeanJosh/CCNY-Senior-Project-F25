using UnityEngine;

public class LaserDetector : MonoBehaviour
{
    [Header("References")]
    public PedestalRotation pedestalRotation;

    public LightReflection mainLaser;
    private Collider myCollider;
    private bool wasHit = false;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        myCollider = GetComponent<Collider>();
        if (mainLaser == null)
        {
            mainLaser = gameObject.AddComponent<LightReflection>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        bool isHit = false;

        if (mainLaser != null && mainLaser.laserPoints.Count > 0)
        {
            Vector3 laserPoint = mainLaser.laserPoints[mainLaser.laserPoints.Count - 1];
            Vector3 closestPoint = myCollider.ClosestPoint(laserPoint);
            float distance = Vector3.Distance(laserPoint, closestPoint);

            if (distance < 0.1f)
            {
                isHit = true;
            }
        }

        if (isHit && !wasHit)
        {
            Debug.Log("Laser detected, triggering rotation");
            if (pedestalRotation != null)
            {
                pedestalRotation.TriggerRotation();
            }
            else
            {
                Debug.LogError("No pedestal rotation assigned");
            }
        }
        else if (!isHit && wasHit)
        {
            Debug.Log("Laser moved away");
        }
        wasHit = isHit;
    }
}
