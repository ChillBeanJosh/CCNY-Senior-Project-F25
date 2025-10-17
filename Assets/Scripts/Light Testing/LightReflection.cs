using NUnit.Framework;
//using UnityEditor.Experimental.GraphView;
using UnityEngine;
using static UnityEngine.UI.Image;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine.Timeline;


[RequireComponent(typeof(LineRenderer))]
public class LightReflection : MonoBehaviour
{
    [Header("Laser Parameters: ")]
    public List<Vector3> laserPoints;
    public RaycastHit[] hits;
    public float lazerDistance;
    private LineRenderer lineRenderer;
    public Material lineMaterial;
    private List<GameObject> laserPointMarkers = new List<GameObject>();
    [Space]

    [Header("Lens Collision: ")]
    public LayerMask lensLayer;
    public bool lensHit;
    [Space]
    [Tooltip("After Colliding, Offsets The Point By X Distance.")]
    public float lazerOffset;
    private Lens lens;
    Vector3 ImagePoint = Vector3.zero;
    private List<Vector3> imagePoints = new List<Vector3>();
    [Space]

    [Header("Prism Collision: ")]
    public LayerMask prismLayer;
    public bool prismHit;
    private Prism prism;
    private List<GameObject> prismSplitBeams = new List<GameObject>();
    private List<GameObject> splitRayMarkers = new List<GameObject>();
    [Space]

    [Header("Burnable Collision: ")]
    public LayerMask burnableLayer;
    public bool burnableHit;
    private Burnable burnable;
    [Space]

    [Header("Debug Visualization")]
    public GameObject obstructionPointMarkerPrefab;
    public GameObject imagePointMarkerPrefab;
    public GameObject endPointMarkerPrefab;
    private List<Vector3> obstructionPoints = new List<Vector3>();
    [Space]



    public float laserWidth;


    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();

        if (laserPoints == null) laserPoints = new List<Vector3>();
    }


    private void Update()
    {
        ClearMarkers();
        ClearPrismSplits();
        ClearSplitRayMarkers();

        //Laser Setup:
        Vector3 ObjectPosition = transform.position;
        Vector3 ObjectDirection = transform.up;
        float remainingLazerDistance = lazerDistance;

        laserPoints.Add(ObjectPosition);
        List<Collider> lensesHit = new List<Collider>();

        Vector3? previousImage = null;

        //Setting a Distance To Avoid Infinite Looping:
        while (remainingLazerDistance > 0f)
        {
            //Ray Setup:
            Ray ray = new Ray(ObjectPosition, ObjectDirection);
            hits = Physics.RaycastAll(ray, remainingLazerDistance, lensLayer | prismLayer | burnableLayer);

            //No Lens Collision + End of Ray:
            if (!ClosestValidHit(hits, lensesHit, out RaycastHit hit))
            {
                Vector3 endPoint = ObjectPosition + ObjectDirection * remainingLazerDistance;
                laserPoints.Add(endPoint);

                if (endPointMarkerPrefab != null)
                {
                    GameObject endMarker = Instantiate(endPointMarkerPrefab, endPoint, Quaternion.identity);
                    laserPointMarkers.Add(endMarker);
                }
                break;
            }

            //Object Refrences:
            lens = hit.collider.GetComponent<Lens>() ?? hit.collider.GetComponentInParent<Lens>();
            prism = hit.collider.GetComponent<Prism>() ?? hit.collider.GetComponentInParent<Prism>();
            burnable = hit.collider.GetComponent<Burnable>() ?? hit.collider.GetComponentInParent<Burnable>();

            //Null Object Checks:
            if (lens == null && prism == null && burnable == null)
            {
                laserPoints.Add(ObjectPosition + ObjectDirection * remainingLazerDistance);
                break;
            }

            //Lens Collison:
            if (lens != null)
            {
                lensHit = true;
                HandleLensHit(hit, lensesHit, ref ObjectPosition, ref ObjectDirection, ref remainingLazerDistance, ref previousImage);
                continue;
            }

            //Prism Collision:
            if (prism != null)
            {
                prismHit = true;
                HandlePrismHit(hit, prism, ObjectDirection, remainingLazerDistance - hit.distance);
                break;
            }

            //Burnable Collision:
            if (burnable != null)
            {
                burnableHit = true;
                HandleBurnableHit(hit);
                break;
            }
        }

        //Function Used to Display Hit Points & Image Points:
        Visualize();
    }

    private void HandleLensHit(RaycastHit hit, List<Collider> lensesHit, ref Vector3 ObjectPosition, ref Vector3 ObjectDirection, ref float remainingLazerDistance, ref Vector3? previousImage)
    {
        lensesHit.Add(hit.collider);
        laserPoints.Add(hit.point);
        obstructionPoints.Add(hit.point);

        Vector3 objectPosForThisLens = previousImage ?? ObjectPosition;

        //When Calculating Image Location:
        if (CalculateImagePoint(objectPosForThisLens, hit.point, lens, out Vector3 calculatedImagePoint))
        {
            //Save Image Point to Use As Object Position For Obstruction:
            previousImage = calculatedImagePoint;

            //Setup Distance Line between hit Location and Image Location:
            Vector3 toImage = calculatedImagePoint - hit.point;
            float toImageDistance = toImage.magnitude;
            Vector3 toImageDir = toImage.normalized;

            //Check For Any Additional Lens Positions Between Hit and Image Positions:
            if (HandleObstructionRecursive(hit.point, toImageDir, toImageDistance, lensesHit, out Vector3 finalImagePoint, out Vector3 nextPosition, out Vector3 nextDirection, out float distanceUsed, previousImage))
            {
                ImagePoint = finalImagePoint;

                ObjectPosition = nextPosition;
                ObjectDirection = nextDirection;

                remainingLazerDistance -= distanceUsed;
                return;
            }

            //No obstruction, Single Lens:
            ImagePoint = calculatedImagePoint;
            imagePoints.Add(ImagePoint);
            laserPoints.Add(ImagePoint);

            ObjectDirection = (ImagePoint - hit.point).normalized;
            ObjectPosition = ImagePoint + ObjectDirection * lazerOffset;

            remainingLazerDistance -= Vector3.Distance(hit.point, ImagePoint);
        }
    }

    private void HandlePrismHit(RaycastHit hit, Prism prism, Vector3 incomingDir, float remainingDistance)
    {
        //Null Check:
        if (prism == null || prism.amountOfSplits <= 0 || prism.range <= 0f || prism.range > 2f * Mathf.PI) return;

        //Contact Point:
        laserPoints.Add(hit.point);
        obstructionPoints.Add(hit.point);

        //Calculate Image Point, To Be Used as Center Point For Prism Split:
        Vector3 centerDir = incomingDir.normalized;
        Vector3 centerImagePoint = hit.point + centerDir * remainingDistance;

        //Calculate Angle Each Ray Is Seperated By:
        float angleStep = prism.range / prism.amountOfSplits;
        float halfRange = prism.range / 2f;

        //For Each Ray:
        for (int i = 0; i < prism.amountOfSplits; i++)
        {
            //Apply the Relative Angle in XY Axis:
            float relativeAngle = -halfRange + i * angleStep;
            Vector3 splitDir = Quaternion.AngleAxis(Mathf.Rad2Deg * relativeAngle, Vector3.forward) * centerDir;

            //Create Empty GameObject, Child of Initial Ray:
            GameObject splitObj = new GameObject($"Split Ray - {i}");
            splitObj.transform.parent = transform;
            splitObj.transform.position = hit.point;
            splitObj.transform.rotation = Quaternion.LookRotation(Vector3.forward, splitDir);

            //Line Render Setup, For Visualization:
            LineRenderer lr = splitObj.AddComponent<LineRenderer>();
            lr.material = lineMaterial != null ? lineMaterial : lineRenderer.material;
            lr.startWidth = laserWidth * 0.8f;
            lr.endWidth = laserWidth * 0.8f;

            //New list, Holds Information of Singular Ray:
            List<Vector3> splitPoints = TraceSplitRay(hit.point, splitDir, remainingDistance);
            lr.positionCount = splitPoints.Count;
            lr.SetPositions(splitPoints.ToArray());

            //Add Ray to List that Holds All Other Split Rays:
            prismSplitBeams.Add(splitObj);
        }
    }

    private List<Vector3> TraceSplitRay(Vector3 origin, Vector3 dir, float maxDistance)
    {
        //[Origin == prism collision point]:
        //[Direction == angle calculated based off angleStep]:
        //[Remaining == current ray distance upon collision]:
        List<Vector3> points = new List<Vector3> { origin };
        Vector3 currentPos = origin;
        Vector3 currentDir = dir;
        float remaining = maxDistance;

        //List holding All Lenses that Collided with current ray (avoids collision issues):
        List<Collider> hitLenses = new List<Collider>();
        Vector3? previousImage = null;

        //While there is still Distance on the Ray:
        while (remaining > 0f)
        {
            //Ray Setup:
            Ray ray = new Ray(currentPos, currentDir);
            RaycastHit[] hits = Physics.RaycastAll(ray, remaining, lensLayer | prismLayer | burnableLayer);

            //No Collision + End of Ray:
            if (!ClosestValidHit(hits, hitLenses, out RaycastHit hit))
            {
                Vector3 endPoint = currentPos + currentDir * remaining;
                points.Add(endPoint);

                //Another Object (Point) at End of Ray (For Visualization):
                if (obstructionPointMarkerPrefab != null)
                {
                    GameObject dataPoint = Instantiate(endPointMarkerPrefab, endPoint, Quaternion.identity);
                    splitRayMarkers.Add(dataPoint);
                }
                break;
            }

            //Object Refrences:
            Lens hitLens = hit.collider.GetComponent<Lens>() ?? hit.collider.GetComponentInParent<Lens>();
            Prism hitPrism = hit.collider.GetComponent<Prism>() ?? hit.collider.GetComponentInParent<Prism>();
            Burnable hitBurnable = hit.collider.GetComponent<Burnable>() ?? hit.collider.GetComponentInParent<Burnable>();


            //Lens Collision:
            if (hitLens != null)
            {
                if (CalculateImagePoint(previousImage ?? currentPos, hit.point, hitLens, out Vector3 imagePoint))
                {
                    points.Add(hit.point);
                    points.Add(imagePoint);

                    //Collision Points:
                    if (obstructionPointMarkerPrefab != null)
                    {
                        GameObject dataPoint = Instantiate(obstructionPointMarkerPrefab, hit.point, Quaternion.identity);
                        splitRayMarkers.Add(dataPoint);
                    }

                    //Image Points:
                    if (imagePointMarkerPrefab != null)
                    {
                        GameObject dataPoint = Instantiate(imagePointMarkerPrefab, imagePoint, Quaternion.identity);
                        splitRayMarkers.Add(dataPoint);
                    }

                    currentPos = imagePoint + (imagePoint - hit.point).normalized * lazerOffset;
                    currentDir = (imagePoint - hit.point).normalized;
                    remaining -= Vector3.Distance(hit.point, imagePoint);
                    previousImage = imagePoint;
                    hitLenses.Add(hit.collider);
                    continue;
                }
            }

            //Prism collision:
            if (hitPrism != null)
            {
                points.Add(hit.point);

                if (obstructionPointMarkerPrefab != null) splitRayMarkers.Add(Instantiate(obstructionPointMarkerPrefab, hit.point, Quaternion.identity));
                HandlePrismHit(hit, hitPrism, currentDir, remaining);

                break;
            }

            //Burnable collision:
            if (hitBurnable != null)
            {
                points.Add(hit.point);

                if (obstructionPointMarkerPrefab != null) splitRayMarkers.Add(Instantiate(obstructionPointMarkerPrefab, hit.point, Quaternion.identity));
                HandleBurnableHit(hit);

                break;
            }

            //Collision with non-lens Surface:
            points.Add(hit.point);
            if (obstructionPointMarkerPrefab != null)
            {
                GameObject dataPoint = Instantiate(obstructionPointMarkerPrefab, hit.point, Quaternion.identity);
                splitRayMarkers.Add(dataPoint);
            }
            break;
        }
        return points;
    }

    private void HandleBurnableHit(RaycastHit hit)
    {
        burnable = hit.collider.GetComponent<Burnable>();
        if (burnable != null)
        {
            burnableHit = true;
            burnable.hitsThisFrame++;

            laserPoints.Add(hit.point);
            obstructionPoints.Add(hit.point);
        }
        else
        {
            burnableHit = false;
        }
    }

    private void ClearMarkers()
    {
        foreach (var marker in laserPointMarkers) Destroy(marker);

        laserPointMarkers.Clear();
        obstructionPoints.Clear();
        imagePoints.Clear();
        laserPoints.Clear();

        lensHit = false;
    }

    private void ClearPrismSplits()
    {
        foreach (var beam in prismSplitBeams) Destroy(beam);
        prismSplitBeams.Clear();

        prismHit = false;
    }

    private void ClearSplitRayMarkers()
    {
        foreach (var marker in splitRayMarkers)
            Destroy(marker);
        splitRayMarkers.Clear();
    }

    private bool ClosestValidHit(RaycastHit[] hitArray, List<Collider> lensesHit, out RaycastHit closestHit)
    {
        //Default Parameters:
        closestHit = default;
        float closestDist = Mathf.Infinity;
        bool found = false;

        //Ensure Raycast Hits can only hit the Same Target Once, But Can Hit Previous Lenses:
        foreach (var hit in hitArray)
        {
            if (lensesHit.Count > 0 && hit.collider == lensesHit[lensesHit.Count - 1]) continue;

            if (hit.distance < closestDist)
            {
                closestDist = hit.distance;
                closestHit = hit;
                found = true;
            }
        }
        return found;
    }

    private bool CalculateImagePoint(Vector3 objectPos, Vector3 hitPoint, Lens lens, out Vector3 imagePoint)
    {
        //Default Image Point:
        imagePoint = Vector3.zero;
        if (lens == null) return false;

        //Focal Length:     [Convex = Positive]     [Concave = Negative]
        float f = lens.isConvex ? Mathf.Abs(lens.focalLength) : -Mathf.Abs(lens.focalLength);

        //Object Distance:
        float p = Vector3.Distance(objectPos, hitPoint);

        //Case to avoid 0:
        if (Mathf.Abs(p) < 0.001f || Mathf.Abs(f - p) < 0.001f) return false;

        //Image Distance:
        float i = 1f / ((1f / f) - (1f / p));

        //Intial Object Height [Based on Hit Point]
        float initialHeight = objectPos.y - hitPoint.y;

        //Magnification:
        float magnification = i / p;

        //Image Height:
        float finalHeight = magnification * initialHeight;


        //Final Image Position:
        Vector3 imageDirection = (i >= 0) ? (hitPoint - objectPos).normalized : -(hitPoint - objectPos).normalized;
        Vector3 baseImagePoint = hitPoint + imageDirection * Mathf.Abs(i);

        imagePoint = new Vector3(baseImagePoint.x, hitPoint.y + finalHeight, baseImagePoint.z);

        return true;
    }


    private bool HandleObstructionRecursive(Vector3 currentHitPoint, Vector3 toImageDir, float toImageDistance, List<Collider> lensesHit, out Vector3 finalImagePoint, out Vector3 nextPosition, out Vector3 nextDirection, out float totalDistanceUsed, Vector3? incomingObjectPoint)
    {
        //Default Parameters for outputs:
        finalImagePoint = Vector3.zero;
        nextPosition = currentHitPoint;
        nextDirection = toImageDir;
        totalDistanceUsed = 0f;


        //Ray Setup:
        Ray obstructionRay = new Ray(currentHitPoint, toImageDir);
        RaycastHit[] obstructionHits = Physics.RaycastAll(obstructionRay, toImageDistance, lensLayer);


        //No Lens Collision: [Return False Since This Function is Used to Check Multiple Lens Collisions]
        if (!ClosestValidHit(obstructionHits, lensesHit, out RaycastHit obstructionHit)) return false;


        //Lens Collison:
        var nextLens = obstructionHit.collider.GetComponent<Lens>() ?? obstructionHit.collider.GetComponentInParent<Lens>();
        if (nextLens == null) return false;


        //Add Lens to a Hit Collection:
        lensesHit.Add(obstructionHit.collider);

        //Add Lens Posistion For Markers:
        obstructionPoints.Add(obstructionHit.point);
        laserPoints.Add(obstructionHit.point);

        Vector3 objectPos = incomingObjectPoint ?? currentHitPoint;

        //When Calculating Image Location:
        if (CalculateImagePoint(objectPos, obstructionHit.point, nextLens, out Vector3 newImagePoint))
        {
            imagePoints.Add(newImagePoint);
            laserPoints.Add(newImagePoint);

            //Set the laser path:
            Vector3 nextDir = (newImagePoint - obstructionHit.point).normalized;
            Vector3 nextPos = newImagePoint + nextDir * lazerOffset;

            //Set the laser remaining Distance:
            float nextDist = Vector3.Distance(obstructionHit.point, newImagePoint);

            //Recursively check for further obstructions down the new path of "newImagePoint":
            if (HandleObstructionRecursive(obstructionHit.point, nextDir, nextDist, lensesHit, out Vector3 deeperImage, out Vector3 deeperPos, out Vector3 deeperDir, out float deeperUsed, newImagePoint))
            {
                finalImagePoint = deeperImage;
                nextPosition = deeperPos;
                nextDirection = deeperDir;
                totalDistanceUsed = Vector3.Distance(currentHitPoint, obstructionHit.point) + deeperUsed;
                return true;
            }
            else
            {
                //No Recursive Checks, 1 Obstruction:
                finalImagePoint = newImagePoint;
                nextPosition = nextPos;
                nextDirection = nextDir;
                totalDistanceUsed = Vector3.Distance(currentHitPoint, newImagePoint);
                return true;
            }
        }

        return false;
    }


    private void Visualize()
    {
        //Visualization of Line Render For Light Source:
        lineRenderer.startWidth = laserWidth;
        lineRenderer.endWidth = laserWidth;

        lineRenderer.positionCount = laserPoints.Count;
        lineRenderer.SetPositions(laserPoints.ToArray());


        //Marker Visualization For Obstruction Points:
        if (obstructionPointMarkerPrefab != null)
        {
            foreach (var point in obstructionPoints)
            {
                GameObject marker = Instantiate(obstructionPointMarkerPrefab, point, Quaternion.identity);
                laserPointMarkers.Add(marker);
            }
        }


        //Marker Visualization For Image Locations:
        if (imagePointMarkerPrefab != null)
        {
            foreach (var point in imagePoints)
            {
                GameObject marker = Instantiate(imagePointMarkerPrefab, point, Quaternion.identity);
                laserPointMarkers.Add(marker);
            }
        }
    }
}
