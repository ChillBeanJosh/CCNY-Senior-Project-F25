using UnityEngine;

public class GhostBottle : MonoBehaviour
{
    bool isCorrectBottle = false;
    bool isTrialActive = false;
    Bottle_GhostTrial bottleTrial;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        bottleTrial = GetComponentInParent<Bottle_GhostTrial>();
    }

    public void SetIsCorrectBottle(bool isCorrect)
    {
        isCorrectBottle = isCorrect;
    }

    private void OnDisable()
    {
        if (isTrialActive)
        {
            bottleTrial.ReceiveChoiceResults(isCorrectBottle);
        }
    }
}
