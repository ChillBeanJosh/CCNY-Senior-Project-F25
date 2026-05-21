using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;


public class MovePlank : MonoBehaviour
{
    public bool activated;
    bool fin;
    [SerializeField] CinemachineCamera shelfCam;

    void Update()
    {
        if (activated && !fin)
        {
            fin = true;
            GameManager.Instance.Player.playerControl = false;
            StartCoroutine(MoveDown());
        }
    }

    IEnumerator MoveDown()
    {
        yield return new WaitForSeconds(0.5f);

        Vector3 start = transform.position;
        Vector3 target = start - Vector3.up * 5.1f;

        float elapsed = 0f;
        float duration = 1f;

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
