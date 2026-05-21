using UnityEngine;
using System.Collections;
using Unity.Cinemachine;


public class FourKeyPlatform : MonoBehaviour
{
    int positionIndex = 0;
    Vector3[] nextPosition = new Vector3[4];
    float moveSpeed = 0.5f;
    [SerializeField] GameObject[] lights;
    [SerializeField] Material lit, unlit;
    [SerializeField] CinemachineCamera coffinCam;
    void Start()
    {
        Vector3 startPos = transform.position;
        for (int i = 0; i < nextPosition.Length; i++)
        {
            nextPosition[i] = startPos + Vector3.down * 2f;
            startPos = nextPosition[i];
        }
    }

    // Called from Shadow Burn
    public void NextThreshold()
    {
        if (positionIndex == 0 || positionIndex == 3) GameManager.Instance.Player.playerControl = false;
        StartCoroutine(TurnOnLight());
    }

    IEnumerator TurnOnLight()
    {
        yield return new WaitForSeconds(0.25f);

        if (positionIndex == 0 || positionIndex == 3) coffinCam.Priority = 20;

        yield return new WaitForSeconds(1.5f);

        lights[positionIndex].GetComponent<Renderer>().material = lit;

        yield return new WaitForSeconds(0.2f);

        lights[positionIndex].GetComponent<Renderer>().material = unlit;

        yield return new WaitForSeconds(0.1f);

        lights[positionIndex].GetComponent<Renderer>().material = lit;

        yield return new WaitForSeconds(0.1f);

        lights[positionIndex].GetComponent<Renderer>().material = unlit;

        yield return new WaitForSeconds(0.1f);

        lights[positionIndex].GetComponent<Renderer>().material = lit;

        yield return new WaitForSeconds(0.05f);

        lights[positionIndex].GetComponent<Renderer>().material = unlit;

        yield return new WaitForSeconds(1f);

        lights[positionIndex].GetComponent<Renderer>().material = lit;

        yield return new WaitForSeconds(1.5f);

        if (positionIndex == 0 || positionIndex == 3)
        {
            coffinCam.Priority = 0;
            GameManager.Instance.Player.playerControl = true;
        }

        positionIndex++;
    }

    IEnumerator MovePlatform()
    {

        lights[positionIndex].GetComponent<Renderer>().material = lit;

        Vector3 start = transform.position;
        Vector3 endPos = nextPosition[positionIndex];


        float elapsed = 0f;
        float duration = Vector3.Distance(start, endPos) / moveSpeed;

        // lerp to target
        while (elapsed < duration)
        {
            transform.position = Vector3.Lerp(start, endPos, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // snap position
        transform.position = endPos;
        positionIndex++;
    }
}
