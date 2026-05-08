using UnityEngine;
using UnityEngine.Events;

public class LevelManager : MonoBehaviour
{
    [Header("Room States")]
    public bool isAcidRoomComplete = false;
    public bool isShadowRoomComplete = false;

    [Header("Center Room Feedback")] 
    [SerializeField] private GameObject acidRoomFire;
    [SerializeField] private GameObject shadowRoomFire;
    
    public UnityEvent OnLevelComplete;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (acidRoomFire != null) acidRoomFire.SetActive(false);
        if (shadowRoomFire != null) shadowRoomFire.SetActive(false);
    }

    public void CompleteAcidRoom()
    {
        if (isAcidRoomComplete) return;
        isAcidRoomComplete = true;
        
        if (acidRoomFire != null) acidRoomFire.SetActive(true);
        CheckLevelCompletion();
    }

    public void CompleteShadowRoom()
    {
        if (isShadowRoomComplete) return;
        isShadowRoomComplete = true;
        
        if (shadowRoomFire != null) shadowRoomFire.SetActive(true);
        CheckLevelCompletion();
    }

    private void CheckLevelCompletion()
    {
        if (isAcidRoomComplete && isShadowRoomComplete)
        {
            OnLevelComplete.Invoke();
        }
    }
}
