using UnityEngine;

public class GhostBottle : MonoBehaviour
{
    [SerializeField] bool isCorrectBottle = false;
    bool isTrialActive = false;
    Bottle_GhostTrial bottleTrial;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        bottleTrial = GetComponentInParent<Bottle_GhostTrial>();
    }

    public void SetBottles(bool isCorrect)
    {
        isCorrectBottle = isCorrect;
    }

    public void SetTrialActive(bool TrialStarted)
    {
        isTrialActive = TrialStarted;
    }
    
    private void OnDisable()
    {
        if (isTrialActive)
        {
            Debug.Log("Bottle Destroyed");
            bottleTrial.ReceiveChoiceResults(isCorrectBottle);
            transform.position = Vector3.zero; // Reset position to zero when the object is disabled
        }
    }
}
