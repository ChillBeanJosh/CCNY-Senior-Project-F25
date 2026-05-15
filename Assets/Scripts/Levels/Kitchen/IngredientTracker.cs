using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class IngredientTracker : MonoBehaviour
{
    public int ingredients = 0;
    [SerializeField] GameObject door;
    bool doorOpened;

    void Update()
    {
        if (ingredients == 2 && !doorOpened)
        {
            doorOpened = true;
            StartCoroutine(OpenDoor());
        }
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.CompareTag("Ingredient"))
        {
            ingredients++;
        }
    }

    IEnumerator OpenDoor()
    {
        Vector3 start = door.transform.position;
        Vector3 endPos = door.transform.position + Vector3.up * 3f;

        float elapsed = 0f;
        float duration = 2f;

        // lerp to target
        while (elapsed < duration)
        {
            door.transform.position = Vector3.Lerp(start, endPos, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // snap position
        door.transform.position = endPos;
    }
}
