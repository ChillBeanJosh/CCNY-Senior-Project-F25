using UnityEngine;

public class BottleManager : MonoBehaviour
{
    public Wobble[] bottles;
    [SerializeField] bool singleObj;
    [SerializeField] Wobble glassObj;
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
            if (singleObj)
            {
                glassObj.wobble = true;
            }
            else
            {
                for (int i = 0; i < bottles.Length; i++)
                {
                    bottles[i].wobble = true;
                }
            }
        }
    }

    void OnTriggerExit(Collider col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            if (singleObj)
            {
                glassObj.wobble = false;
            }
            else
            {
                for (int i = 0; i < bottles.Length; i++)
                {
                    bottles[i].wobble = false;
                }
            }
        }
    }
}
