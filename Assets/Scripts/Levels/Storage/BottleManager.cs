using UnityEngine;

public class BottleManager : MonoBehaviour
{
    public Wobble[] bottles;
    void Start()
    {
        bottles = new Wobble[transform.childCount];

        for (int i = 0; i < bottles.Length; i++)
        {
            bottles[i] = transform.GetChild(i).GetComponent<Wobble>();
        }
    }

    void Update()
    {

    }

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            for (int i = 0; i < bottles.Length; i++)
            {
                bottles[i].wobble = true;
            }
        }
    }

    void OnTriggerExit(Collider col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            for (int i = 0; i < bottles.Length; i++)
            {
                bottles[i].wobble = false;
            }
        }
    }
}
