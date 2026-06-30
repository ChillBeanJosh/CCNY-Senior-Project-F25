using UnityEngine;

public class StartTrial : MonoBehaviour
{
    [SerializeField] GameObject[] NormalObject;
    [SerializeField] GameObject[] SpectralObject;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            for (int i = 0; i < NormalObject.Length; i++)
                NormalObject[i].SetActive(false);

            for (int i = 0; i < SpectralObject.Length; i++)
                SpectralObject[i].SetActive(true);
        }
    }
}
