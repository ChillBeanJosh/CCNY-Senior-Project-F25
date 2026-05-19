using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
public class ShadowCaster : MonoBehaviour
{
    public bool isChecking = true;
    public int requiredPlayers = 1;
    [SerializeField] Transform player;
    [SerializeField] List<Transform> players;
    [SerializeField] float castingTime, currentCastingTime = 0f;
    bool castComplete;
    [Space(15)]
    [Header("Door")]
    [SerializeField] bool hasDoor;
    [SerializeField] Transform door;
    [SerializeField] Transform doorTarget;
    [SerializeField] float doorSpeed;
    [SerializeField] float doorDelay = 0f;
    public bool doorOpened;
    [SerializeField] bool moveDoor;

    [Space]
    [SerializeField] bool committalLevel;
    [SerializeField] ShadowDetection shadowDetection;
    [Space]
    [SerializeField] ShadowPuzzleTrigger shadowTrigger;
    [SerializeField] GameObject shadowPrefab;
    GameObject shadow;
    [SerializeField] Transform shadowTarget;
    [SerializeField] bool doubleShadow;
    [SerializeField] Transform thinShadowTarget;

    void Start()
    {
        if (player == null)
            player = GameManager.Instance.Player.GetComponentInChildren<Light>().transform;

        players.Add(player);
    }

    void Update()
    {
        if (!isChecking) return;

        // Update reference to current player when switching characters
        Transform currentPlayer = GameManager.Instance.Player.GetComponentInChildren<Light>().transform;
        if (currentPlayer != player)
        {
            if (players.Count == 1)
                players.Add(currentPlayer);

            player = currentPlayer;
        }

        if (castComplete)
        {
            if (shadowPrefab != null && !shadowDetection.completed)
            {
                // Instantiate shadow at burn site when burn is complete
                if (!doubleShadow)
                {
                    GameObject playerShadow = GameManager.Instance.Player.gameObject.GetComponent<DrawShadows>().shadow;
                    shadow = shadowPrefab;
                    shadow.transform.localScale = playerShadow.transform.localScale;
                    Instantiate(shadow, playerShadow.transform.position, playerShadow.transform.rotation);
                }
                else
                {
                    Instantiate(shadowPrefab, thinShadowTarget.position, thinShadowTarget.rotation);
                }

                // Lower coffin 
                if (committalLevel) GameObject.Find("Coffin").GetComponent<FourKeyPlatform>().NextThreshold();

                shadowDetection.completed = true;
            }


            if (hasDoor && !moveDoor)
            {
                moveDoor = true;
                StartCoroutine(OpenDoor(doorTarget.position));
            }

            return;
        }

        if (shadowDetection.shadowIsInside)
        {
            if (currentCastingTime != 1f)
            {
                ShadowCasting(true);
            }
            else
            {
                castComplete = true;
            }

        }
        else
        {
            if (currentCastingTime > 0f)
            {
                ShadowCasting(false);
            }
            else
            {
                currentCastingTime = 0f;
            }

        }
    }

    void ShadowCasting(bool inPosition)
    {
        float time = Time.deltaTime / castingTime;

        if (inPosition)
            currentCastingTime += time;
        else
            currentCastingTime -= time;
        currentCastingTime = Mathf.Clamp01(currentCastingTime);
    }

    IEnumerator OpenDoor(Vector3 target)
    {
        yield return new WaitForSeconds(doorDelay);

        Vector3 start = door.position;
        Vector3 endPos = target;

        float elapsed = 0f;
        float duration = doorSpeed;

        // lerp to target
        while (elapsed < duration)
        {
            door.position = Vector3.Lerp(start, endPos, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // snap position
        door.position = endPos;
    }
}

