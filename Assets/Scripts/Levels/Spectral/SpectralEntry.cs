using UnityEngine;

public class SpectralEntry : MonoBehaviour
{
    [SerializeField] GameObject Player;
    [SerializeField] Vector3 TeleportLocation;
    [SerializeField] GameObject SpectralArea;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Player = GetComponent<GameObject>();
    }

    private void OnDestroy()
    {
        //transport player to designated location
        if (SpectralArea != null) SpectralArea.SetActive(true);
        if (TeleportLocation != null && Player != null) Player.transform.position = TeleportLocation;
    }
}
