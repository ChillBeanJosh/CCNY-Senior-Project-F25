using UnityEngine;
using System.Collections;

public class MoveLantern : MonoBehaviour
{
    [SerializeField] Transform target;
    OscillateObject floatingObj;
    bool inPosition;
    public bool move, turnOnDebrisOscillation;

    void Start()
    {
        floatingObj = GetComponent<OscillateObject>();
    }

    void Update()
    {
        //if (burnable == null && !inPosition) StartCoroutine(MoveToPosition(target.position));

        if (move)
        {
            move = false;
            StartCoroutine(MoveToTarget(target.position));
        }
    }


    IEnumerator MoveToTarget(Vector3 target)
    {
        floatingObj.pause = true;

        Vector3 start = transform.position;
        Vector3 end = target;

        float elapsed = 0f;
        float duration = 3f;

        // lerp to target
        while (elapsed < duration)
        {
            transform.position = Vector3.Lerp(start, end, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = end;
        floatingObj.pos = floatingObj.transform.position;
        floatingObj.pause = false;
        turnOnDebrisOscillation = true;
    }
    //Thanks, Josh!
    IEnumerator MoveToPosition(Vector3 target)
    {
        Vector3 startPos = transform.position;
        Vector3 endPos = target;

        float elapsed = 0f;
        float duration = Vector3.Distance(startPos, endPos) / 5.0f;

        // lerp to target
        while (elapsed < duration)
        {
            transform.position = Vector3.Lerp(startPos, endPos, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // snap position
        transform.position = endPos;
        //GetComponentInChildren<SphereCollider>().enabled = true;
        inPosition = true;
    }
}
