using UnityEngine;

public class TwoPlayerShadowTracker : MonoBehaviour
{
    [SerializeField] ShadowDetection shadowDetection;

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            shadowDetection.AddPlayer(col.gameObject);
        }
    }

    void OnTriggerExit(Collider col)
    {
        if (col.gameObject.CompareTag("Player"))
        {

        }
    }
}
