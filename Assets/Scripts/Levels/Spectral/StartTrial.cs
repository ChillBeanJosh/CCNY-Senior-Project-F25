using UnityEngine;

public class StartTrial : MonoBehaviour
{
    [SerializeField] GameObject SpectralObject;
    [SerializeField] TrialManager trialManager;
    bool canStartTrial = true;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && canStartTrial)
        {
            canStartTrial = false;
            trialManager.UpdateTrialStatus(true);
            SpectralObject.SendMessage("StartTrial", true);
            this.gameObject.SetActive(false);
        }
    }

    public void SetCanStartTrial(bool value)
    {
        canStartTrial = value;
    }
}
