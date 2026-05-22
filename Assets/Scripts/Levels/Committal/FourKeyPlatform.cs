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
    [SerializeField] CinemachineCamera coffinCam, coffinCam2;
    [SerializeField] Animator debrisAnim;
    [SerializeField] OscillateObject[] debrisOscillation;
    [SerializeField] MoveLantern moveLantern;
    [SerializeField] Transform coffin, target;

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
        GameManager.Instance.Player.playerControl = false;
        StartCoroutine(TurnOnLight());
    }

    IEnumerator TurnOnLight()
    {
        yield return new WaitForSeconds(0.25f);

        coffinCam.Priority = 20;

        yield return new WaitForSeconds(1.5f);

        if (positionIndex == 1)
        {
            for (int i = 0; i < debrisOscillation.Length; i++)
            {
                debrisOscillation[i].pause = true;
                debrisOscillation[i].transform.position = debrisOscillation[i].pos;
            }
        }

        lights[positionIndex].GetComponent<Renderer>().material = lit;

        yield return new WaitForSeconds(0.15f);

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

        if (positionIndex == 1)
        {
            Invoke(nameof(MoveDebris), 0.8f);
        }

        coffinCam.Priority = 0;

        if (positionIndex == 3)
        {
            coffinCam2.Priority = 20;
            StartCoroutine(MovePlatform());
        }
        else
        {
            GameManager.Instance.Player.playerControl = true;

            positionIndex++;
        }
    }

    IEnumerator MovePlatform()
    {

        //lights[positionIndex].GetComponent<Renderer>().material = lit;

        Vector3 start = coffin.transform.position;
        Vector3 end = target.position; //nextPosition[positionIndex];


        float elapsed = 0f;
        float duration = 6f;

        // lerp to target
        while (elapsed < duration)
        {
            coffin.transform.position = Vector3.Lerp(start, end, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // snap position
        coffin.transform.position = end;

        yield return new WaitForSeconds(1f);

        coffinCam2.Priority = 0;
        GameManager.Instance.Player.playerControl = true;

    }

    void MoveDebris()
    {
        debrisAnim.SetTrigger("Move");
        moveLantern.move = true;
    }
}
