using UnityEngine;

public class Cauldron : MonoBehaviour
{
    public enum RoomType { Acid, Shadow }

    [Header("Cauldron Settings")] 
    public RoomType roomType;

    [SerializeField] private GameObject cauldronFire;

    private bool isLit = false;
    private LevelManager levelManager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (cauldronFire != null) cauldronFire.SetActive(false);
        levelManager = FindFirstObjectByType<LevelManager>();
        
    }

    public void LightCauldron()
    {
        if (isLit) return;
        isLit = true;
        if (cauldronFire != null) cauldronFire.SetActive(true);

        if (levelManager != null)
        {
            if (roomType == RoomType.Acid)
                levelManager.CompleteAcidRoom();
            else if (roomType == RoomType.Shadow)
                levelManager.CompleteShadowRoom();
        }
    }
}
