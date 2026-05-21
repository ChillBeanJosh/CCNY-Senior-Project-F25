using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MovePlank : MonoBehaviour
{
    public bool activated;
    bool fin;

    void Update()
    {
        if (activated && !fin)
        {
            fin = true;
            StartCoroutine(MoveDown());
        }
    }

    IEnumerator MoveDown()
    {
        Vector3 start = transform.position;
        Vector3 target = start - Vector3.up * 5.1f;

        float elapsed = 0f;
        float duration = 1.0f;

        while (elapsed < duration)
        {
            float time = elapsed / duration;
            transform.position = Vector3.Lerp(start, target, time);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = target;
    }
}
