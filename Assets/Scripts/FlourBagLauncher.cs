using UnityEngine;
using MoreMountains.Feedbacks;

public class FlourBagLauncher : MonoBehaviour
{
    [SerializeField] private MMF_Player sequencePlayer;
    [SerializeField] private bool plankDestroyed = false;
    [SerializeField] private ExitUnlock exitUnlock;

    private bool _sequenceTriggered = false;

    public void SetPlankAsDestroyed()
    {
        plankDestroyed = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (plankDestroyed && !_sequenceTriggered)
            {
                TriggerSequence();
            }
        }
    }

    private void TriggerSequence()
    {
        _sequenceTriggered = true;
        
        if (exitUnlock != null)
        {
            exitUnlock.AddShelfIngredient();
        }
        
        if (sequencePlayer != null)
        {
            sequencePlayer.PlayFeedbacks();
        }
    }

}
