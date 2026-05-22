using UnityEngine;

public class HideTutorial : MonoBehaviour
{
    [SerializeField] GameObject tutorial;

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            tutorial.SetActive(false);
        }
    }
}
