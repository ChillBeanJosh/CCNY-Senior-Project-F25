using UnityEngine;

public class StartTrial : MonoBehaviour
{
    [SerializeField] GameObject SpectralObject;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SpectralObject.SendMessage("StartTrial", true);
            this.gameObject.SetActive(false);
        }
    }
}
