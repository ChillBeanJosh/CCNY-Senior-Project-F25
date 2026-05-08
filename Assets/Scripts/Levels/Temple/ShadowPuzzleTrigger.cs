using UnityEngine;

public class ShadowPuzzleTrigger : MonoBehaviour
{
    public bool detectPlayer = true;
    [SerializeField] Transform[] corners;
    [SerializeField] bool hasBurnables;
    [SerializeField] Vector3 orientation;
    [SerializeField] GameObject shadowPrefab;
    [Header("Two Player Check")]
    [Tooltip("Leave empty if puzzle requires only one shadow.")]
    [SerializeField] ShadowDetection shadowDetection;


    void Start()
    {
        int burnables = hasBurnables ? 1 : 0;
        corners = new Transform[transform.childCount - burnables];

        for (int i = 0; i < corners.Length; i++)
        {
            corners[i] = transform.GetChild(i);
        }
    }
    void OnTriggerEnter(Collider col)
    {
        if (detectPlayer)
        {
            if (col.gameObject.CompareTag("Player"))
            {
                DrawShadows drawShadows = col.gameObject.GetComponent<DrawShadows>();
                drawShadows.box = this.transform;
                drawShadows.boxCorners = corners;
                drawShadows.shadowOrientation = orientation;
                drawShadows.CreateShadow(shadowPrefab);
                drawShadows.shadowPuzzleActive = true;
            }
        }
    }

    void OnTriggerExit(Collider col)
    {
        if (detectPlayer)
        {
            if (col.gameObject.CompareTag("Player"))
            {
                DrawShadows drawShadows = col.gameObject.GetComponent<DrawShadows>();
                drawShadows.shadowOrientation = Vector3.zero;
                drawShadows.box = null;
                drawShadows.boxCorners = null;
                if (shadowDetection != null) shadowDetection.RemoveShadowFromList(col);
                drawShadows.shadowPuzzleActive = false;
            }
        }
    }
}
